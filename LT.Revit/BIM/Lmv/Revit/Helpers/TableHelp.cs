using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using BIM.Lmv.Content.Geometry.Types;
using BIM.Lmv.Types;
using Newtonsoft.Json.Linq;
using Utils;
using Transform = Autodesk.Revit.DB.Transform;

namespace BIM.Lmv.Revit.Helpers
{
    internal class TableHelp
    {
        public static Dictionary<string, string> _configDic = new Dictionary<string, string>();
        public static Vector3F _EleBoxMax = new Vector3F(float.MinValue, float.MinValue, float.MinValue);
        public static Vector3F _EleBoxMin = new Vector3F(float.MaxValue, float.MaxValue, float.MaxValue);
        public static string _sCurBFileId;
        public static string _sDirPath;
        public static string _sPathData;
        public static string _sPathData2;
        public static string _sProjectPrefix;
        public static SqliteOpr _SqliteOpr = new SqliteOpr();//sql操作器
        public static string _sUrlRoot;//上传文件
        public static string _sZipKey;
        public static string _sZipPath = "";
        public static bool g_bFree = false;
        public static string g_modelName;//模型名称
        public static string g_modelTypeNo;//项目名称
        public static string g_stage;//项目阶段
        public static string sUser;
        public bool _bIsInstance;
        public Dictionary<string, string> _FamilyIns;
        public int _InsNodeIdDelay;
        public Dictionary<int, Vector3F> _InstanceBoxMax;
        public Dictionary<int, Vector3F> _InstanceBoxMin;
        public Dictionary<int, string> _InstanceTemplates;
        public string _sBlockId = "";
        public string _sMatrix = "";
        public SceneInfo _SneneInfo;
        public Vector3F _TLGlobalBoxMax = new Vector3F(float.MinValue, float.MinValue, float.MinValue);
        public Vector3F _TLGlobalBoxMin = new Vector3F(float.MaxValue, float.MaxValue, float.MaxValue);
        public Transform _TransformDelay;

        public TableHelp(Document doc)
        {
            _SqliteOpr.connectToDatabase(_sPathData);
            _SneneInfo = ExportHelper.GetSceneInfo(doc);
            _TransformDelay = Transform.Identity;
            _InstanceTemplates = new Dictionary<int, string>();
            _InstanceBoxMin = new Dictionary<int, Vector3F>();
            _InstanceBoxMax = new Dictionary<int, Vector3F>();
            _FamilyIns = new Dictionary<string, string>();
            _sCurBFileId = Guid.NewGuid().ToString();
        }

        public void AddProject(string strSourceFile)
        {
            try
            {
                var sID = _sProjectPrefix.Substring(1);
                _sZipKey = Guid.NewGuid().ToString();
                var str2 = Guid.NewGuid().ToString();
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(strSourceFile);
                var str4 = g_modelName;
                var extension = Path.GetExtension(strSourceFile);
                var sUser = TableHelp.sUser;
                var str7 = g_modelTypeNo;
                var sColume = "NAME,TYPE,AFFIXFILEID";
                var sValue = "'" + fileNameWithoutExtension + "','" + extension + "','" + str2 + "'";
                var str10 = _SqliteOpr.fillTable("AFFIXTABLE" + _sProjectPrefix, sColume, sValue, "");
                sColume = "NAME,PREFIXION,SOURCEID,ZIPKEY,FOREIGNID,DESCRIBE";
                sValue = "'" + str4 + "'," + sID + "," + str10 + ",'" + _sZipKey + "','" + str7 + "','" + sUser + "'";
                _SqliteOpr.fillTable("PROJECT", sColume, sValue, sID);
            }
            catch (Exception)
            {
            }
        }

        public void ClearEleBox()
        {
            _EleBoxMin.set(float.MaxValue, float.MaxValue, float.MaxValue);
            _EleBoxMax.set(float.MinValue, float.MinValue, float.MinValue);
        }

        private static void delDir(string strFile)
        {
            new Thread(delDirFunc) {IsBackground = true}.Start(strFile);
        }

        public static void delDirFunc(object data)
        {
            try
            {
                var path = data as string;
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                }
            }
            catch (Exception)
            {
            }
        }

        public static void delFile(string strFile)
        {
            new Thread(delFileFunc) {IsBackground = true}.Start(strFile);
        }

        public static void delFileFunc(object data)
        {
            var path = data as string;
            if (path.Contains("e.db"))
            {
                while (!g_bFree)
                {
                    Thread.Sleep(0x3e8);
                }
                Thread.Sleep(0x7d0);
            }
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch (Exception)
            {
            }
        }

        public void ElementBegin()
        {
            ClearEleBox();
            _sBlockId = "";
            _bIsInstance = false;
            TLGeometryHelper.strMeshIds = "";
        }

        public void ElementEnd(bool bHasGeometry)
        {
            if (bHasGeometry && (_EleBoxMin != null) && (_EleBoxMin.x != float.MaxValue))
            {
                _TLGlobalBoxMin.x = Math.Min(_TLGlobalBoxMin.x, _EleBoxMin.x);
                _TLGlobalBoxMin.y = Math.Min(_TLGlobalBoxMin.y, _EleBoxMin.y);
                _TLGlobalBoxMin.z = Math.Min(_TLGlobalBoxMin.z, _EleBoxMin.z);
                _TLGlobalBoxMax.x = Math.Max(_TLGlobalBoxMax.x, _EleBoxMax.x);
                _TLGlobalBoxMax.y = Math.Max(_TLGlobalBoxMax.y, _EleBoxMax.y);
                _TLGlobalBoxMax.z = Math.Max(_TLGlobalBoxMax.z, _EleBoxMax.z);
            }
        }

        public string ElemWriteTable(Element curElem, Document tDoc, Transform tTransform)
        {
            var str = "";
            if (_bIsInstance)
            {
                InstanceEndDelay();
            }
            if (!string.IsNullOrEmpty(TLGeometryHelper.strMeshIds) || !string.IsNullOrEmpty(_sBlockId))
            {
                var str2 = "";
                var typeId = curElem.GetTypeId();
                var element = tDoc.GetElement(typeId);
                if ((typeId != null) && (element != null))
                {
                    string str3;
                    if (_FamilyIns.TryGetValue(typeId.ToString(), out str3))
                    {
                        str2 = str3;
                    }
                    else
                    {
                        var name = curElem.Category.Name;
                        var sFamilyName = name;
                        var sName = curElem.Name;
                        var type = element as ElementType;
                        if (type != null)
                        {
                            sFamilyName = type.FamilyName;
                            sName = type.Name;
                            if (type.Category != null)
                            {
                                name = type.Category.Name;
                            }
                        }
                        str2 = WriteFamilyTable(element.Parameters, sName, sFamilyName, name, typeId.ToString());
                        _FamilyIns.Add(typeId.ToString(), str2);
                    }
                }
                var uniqueId = curElem.UniqueId;
                var title = tDoc.Title;
                var sBox = string.Format("{0:F3},{1:F3},{2:F3},{3:F3},{4:F3},{5:F3}", _EleBoxMin.x, _EleBoxMin.y,
                    _EleBoxMin.z, _EleBoxMax.x, _EleBoxMax.y, _EleBoxMax.z);
                if (_bIsInstance)
                {
                    var geoItemt = new GeoItem(uniqueId, _sMatrix, "null", _sBlockId, sBox, title, str2);
                    str = WriteGeoItem(geoItemt);
                }
                else
                {
                    var matrixFrom = TransformHelper.GetMatrixFrom(tTransform);
                    var v = new Vector3F((float) tTransform.Origin.X, (float) tTransform.Origin.Y,
                        (float) tTransform.Origin.Z);
                    matrixFrom.setPosition(v);
                    _sMatrix = matrixFrom.toString();
                    var item2 = new GeoItem(uniqueId, _sMatrix, TLGeometryHelper.strMeshIds, "-1", sBox, title,
                        str2);
                    str = WriteGeoItem(item2);
                }
                TLGeometryHelper.strMeshIds = "";
            }
            return str;
        }

        public static string exeRequest(string sUrlseg, string sMethod, string sData, ref string sRetData,
            bool bFirst = false)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var str = "success";
            var request = WebRequest.Create(_sUrlRoot + sUrlseg) as HttpWebRequest;
            request.Timeout = 0x493e0;
            request.Method = sMethod;
            request.ContentType = "application/json;charset=UTF-8";
            if ((sMethod == "POST") && !string.IsNullOrEmpty(sData))
            {
                var bytes = Encoding.UTF8.GetBytes(sData);
                request.ContentLength = bytes.Length;
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(bytes, 0, bytes.Length);
                }
            }
            using (var response = request.GetResponse() as HttpWebResponse)
            {
                var obj2 = JObject.Parse(new StreamReader(response.GetResponseStream(), Encoding.ASCII).ReadToEnd());
                str = obj2["success"].ToString();
                if (!string.IsNullOrEmpty(sRetData))
                {
                    if (bFirst)
                    {
                        sRetData = obj2[sRetData].ToString();
                    }
                    else
                    {
                        sRetData = obj2["data"].Value<string>(sRetData);
                    }
                }
            }
            stopwatch.Stop();
            var elapsed = stopwatch.Elapsed;
            return str;
        }

        public void Finish()
        {
            var geoItemt = new CoordinaeSysItem
            {
                Name = "geosys",
                False_easting = _SneneInfo.Longitude.ToString(),//长度
                False_northing = _SneneInfo.Latitude.ToString(),//高度
                Centralmeridian = _SneneInfo.AngleToTrueNorth.ToString()//中心顶点
            };
            WriteCoordSysItem(geoItemt);
            try
            {
                if (File.Exists(_sZipPath))
                {
                    var bArray = File.ReadAllBytes(_sZipPath);
                    var sColume = "OBJGUID";
                    var sValue = "'" + _sZipKey + "'";
                    var sObjectId = _SqliteOpr.fillTable("AFFIXFILE" + _sProjectPrefix, sColume, sValue, "");
                    _SqliteOpr.fillTableblob("AFFIXFILE" + _sProjectPrefix, sObjectId, "CONTENT", bArray);
                }
            }
            catch (Exception)
            {
            }
            _SqliteOpr.commitData();
            _SqliteOpr = null;
            try
            {
                if (File.Exists(_sPathData))
                {
                    File.Copy(_sPathData, _sPathData2, true);
                    Thread.Sleep(0x3e8);
                    var bArr = File.ReadAllBytes(_sPathData2);
                    UploadFileXss("insertBatch?Prefixion=" + _sProjectPrefix, bArr);
                }
            }
            catch (Exception)
            {
            }
            try
            {
                delFile(_sZipPath);
                delDir(_sDirPath);
                delFile(_sPathData);
                delFile(_sPathData2);
            }
            catch (Exception)
            {
            }
            _sZipPath = "";
            _sPathData = "";
            try
            {
                g_modelName = HttpUtility.UrlEncode(g_modelName, Encoding.GetEncoding("utf-8"));
                var str4 = _sProjectPrefix.Substring(1);
                var dictParam = new Dictionary<string, string>
                {
                    {
                        "pro_no",
                        g_modelTypeNo
                    },
                    {
                        "mod_typ",
                        g_stage
                    },
                    {
                        "mod_nam",
                        g_modelName
                    },
                    {
                        "mod_id",
                        str4
                    }
                };
                new WebServiceClient().Post("uploadModel", dictParam);
                g_modelTypeNo = "";
                g_modelName = "";
                g_stage = "";
            }
            catch (Exception)
            {
            }

            //运行完成了， 报出消息
            MessageBox.Show("数据上传完成！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

        public string HandleInstanceTable(int nKey)
        {
            var sName = nKey.ToString();
            var sDesc = sName + "_Instance";
            var geoItemt = new GeoBlockItem(sName, sDesc, TLGeometryHelper.strMeshIds);
            var str3 = WriteGeoInstanceItem(geoItemt);
            TLGeometryHelper.strMeshIds = "";
            return str3;
        }

        public static void InitConfig()
        {
            try
            {
                var path = @"C:\temp\config.txt";
                if (!File.Exists(path))
                {
                    MessageBox.Show(path + "---:配置文件不存在!");
                }
                else
                {
                    string str2;
                    var stream = new FileStream(path, FileMode.OpenOrCreate);
                    var reader = new StreamReader(stream);
                    while ((str2 = reader.ReadLine()) != null)
                    {
                        var index = str2.IndexOf('=');
                        _configDic.Add(str2.Substring(0, index).Trim(), str2.Substring(index + 1).Trim());
                    }
                    reader.Close();
                    stream.Close();
                }
            }
            catch (Exception)
            {
            }
        }

        public bool InstanceBegin(Transform tTransform, int nSymbolID)
        {
            string str;
            _bIsInstance = true;
            var matrixFrom = TransformHelper.GetMatrixFrom(tTransform);
            var v = new Vector3F((float) tTransform.Origin.X, (float) tTransform.Origin.Y,
                (float) tTransform.Origin.Z);
            matrixFrom.setPosition(v);
            _sMatrix = matrixFrom.toString();
            if (_InstanceTemplates.TryGetValue(nSymbolID, out str))
            {
                _sBlockId = str;
                return true;
            }
            return false;
        }

        public void InstanceEnd(Transform tTransform, int nSymbolID)
        {
            _InsNodeIdDelay = nSymbolID;
            _TransformDelay = tTransform;
        }

        public void InstanceEndDelay()
        {
            string str;
            var flag = false;
            if (!_InstanceTemplates.TryGetValue(_InsNodeIdDelay, out str))
            {
                if (!string.IsNullOrEmpty(TLGeometryHelper.strMeshIds))
                {
                    _sBlockId = HandleInstanceTable(_InsNodeIdDelay);
                    _InstanceTemplates.Add(_InsNodeIdDelay, _sBlockId);
                    _InstanceBoxMin.Add(_InsNodeIdDelay, _EleBoxMin);
                    _InstanceBoxMax.Add(_InsNodeIdDelay, _EleBoxMax);
                    flag = true;
                }
            }
            else
            {
                flag = true;
            }
            if (flag)
            {
                var matrixFrom = TransformHelper.GetMatrixFrom(_TransformDelay);
                var v = new Vector3F((float) _TransformDelay.Origin.X, (float) _TransformDelay.Origin.Y,
                    (float) _TransformDelay.Origin.Z);
                matrixFrom.setPosition(v);
                _EleBoxMin = _InstanceBoxMin[_InsNodeIdDelay];
                _EleBoxMax = _InstanceBoxMax[_InsNodeIdDelay];
                _EleBoxMin.applyMatrix4(matrixFrom);
                _EleBoxMax.applyMatrix4(matrixFrom);
            }
        }

        public static void OnWiteFamilyTable(List<FamilyItem> ListData, string sFamilyName, string sFamilyID)
        {
        }

        public static void OnWitePropertyTable(List<PropertyItem> ListData, string strElementType, string strGeoID)
        {
            try
            {
                foreach (var item in ListData)
                {
                    var sColume = "SECTION,NAME,VALUE,UNIT,TYPE,GEOID";
                    var sValue = "'" + item.Section + "','" + item.Name + "','" + item.Value + "','" + item.Unit +
                                 "'," + item.Type + "," + strGeoID;
                    _SqliteOpr.fillTable("PROTABLE" + _sProjectPrefix, sColume, sValue, "");
                }
            }
            catch (Exception)
            {
            }
        }

        public static string Upload_Request(string sUrlseg, string fileNamePath, BinaryReader brIn = null,
            long nFileLength = 0L)
        {//上传文件
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var requestUriString = _sUrlRoot + sUrlseg;
            var str2 = "";
            var str3 = "Binaryfile";
            var str4 = "----------" + DateTime.Now.Ticks.ToString("x");
            var bytes = Encoding.ASCII.GetBytes("\r\n--" + str4 + "\r\n");
            var builder = new StringBuilder();
            builder.Append("--");
            builder.Append(str4);
            builder.Append("\r\n");
            builder.Append("Content-Disposition: form-data; name=\"");
            builder.Append("file");
            builder.Append("\"; filename=\"");
            builder.Append(str3);
            builder.Append("\"");
            builder.Append("\r\n");
            builder.Append("Content-Type: ");
            builder.Append("application/octet-stream");
            builder.Append("\r\n");
            builder.Append("\r\n");
            var s = builder.ToString();
            var buffer = Encoding.UTF8.GetBytes(s);
            var length = nFileLength;
            var reader = brIn;
            FileStream input = null;
            if (length == 0L)
            {
                input = new FileStream(fileNamePath, FileMode.Open, FileAccess.Read);
                reader = new BinaryReader(input);
                length = input.Length;
            }
            var request = (HttpWebRequest) WebRequest.Create(requestUriString);
            request.Method = "POST";
            request.AllowWriteStreamBuffering = false;
            request.Timeout = 0x1e8480;
            request.ContentType = "multipart/form-data; boundary=" + str4;
            var num2 = length + buffer.Length + bytes.Length;
            request.ContentLength = num2;
            try
            {
                var requestStream = request.GetRequestStream();
                requestStream.Write(buffer, 0, buffer.Length);
                var count = Math.Min(0x7d000, (int) length);
                var buffer3 = new byte[count];
                for (var i = reader.Read(buffer3, 0, count); i > 0; i = reader.Read(buffer3, 0, count))
                {
                    requestStream.Write(buffer3, 0, i);
                }
                requestStream.Write(bytes, 0, bytes.Length);
                var responseStream = request.GetResponse().GetResponseStream();
                var reader2 = new StreamReader(responseStream);
                str2 = reader2.ReadLine();
                responseStream.Close();
                reader2.Close();
            }
            catch (Exception exception)
            {
                str2 = exception.ToString();
            }
            if (input != null)
            {
                input.Close();
                input.Dispose();
            }
            stopwatch.Stop();
            var elapsed = stopwatch.Elapsed;
            return str2;
        }

        public static string UploadFileXss(string sUrlseg, byte[] bArr)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var requestUriString = _sUrlRoot + sUrlseg;
            var str2 = "";
            var str3 = "Binaryfile";
            try
            {
                var request = WebRequest.Create(requestUriString) as HttpWebRequest;
                request.Timeout = 0x1e8480;
                request.Method = "POST";
                var str4 = DateTime.Now.Ticks.ToString("X");
                request.ContentType = "multipart/form-data;charset=utf-8;boundary=" + str4;
                var bytes = Encoding.UTF8.GetBytes("\r\n--" + str4 + "\r\n");
                var buffer = Encoding.UTF8.GetBytes("\r\n--" + str4 + "--\r\n");
                var builder =
                    new StringBuilder(
                        string.Format(
                            "Content-Disposition:form-data;name=\"file\";filename=\"{0}\"\r\nContent-Type:application/octet-stream\r\n\r\n",
                            str3));
                var buffer3 = Encoding.UTF8.GetBytes(builder.ToString());
                var requestStream = request.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Write(buffer3, 0, buffer3.Length);
                requestStream.Write(bArr, 0, bArr.Length);
                requestStream.Write(buffer, 0, buffer.Length);
                requestStream.Close();
                var response = request.GetResponse() as HttpWebResponse;
                str2 = new StreamReader(response.GetResponseStream(), Encoding.UTF8).ReadToEnd();
            }
            catch (Exception exception)
            {
                str2 = exception.ToString();
            }
            stopwatch.Stop();
            var elapsed = stopwatch.Elapsed;
            return str2;
        }

        public static string WriteCoordSysItem(CoordinaeSysItem geoItemt)
        {
            var str = "";
            try
            {
                var sColume = "NAME,FALSE_EASTING,FALSE_NORTHING,CENTRALMERIDIAN";
                var sValue = "'" + geoItemt.Name + "'," + geoItemt.False_easting + "," + geoItemt.False_northing +
                             "," + geoItemt.Centralmeridian;
                str = _SqliteOpr.fillTable("COORDINATE_SYSTEM", sColume, sValue, "");
            }
            catch (Exception)
            {
            }
            return str;
        }

        public static string WriteFamilyTable(ParameterSet Parameters, string sName, string sFamilyName,
            string sCategory, string sDesc)
        {
            var str = "";
            var sColume = " ";
            var sValue = " ";
            try
            {
                var str4 = _SqliteOpr.getTableField("FAMILYTABLE" + _sProjectPrefix, "OBJECTID",
                    "NAME == '" + sCategory + "'");
                if (string.IsNullOrEmpty(str4))
                {
                    sColume = "NAME,PARENTID,DESCRIBE";
                    sValue = "'" + sCategory + "',-1,'" + sDesc + "'";
                    str4 = _SqliteOpr.fillTable("FAMILYTABLE" + _sProjectPrefix, sColume, sValue, "");
                }
                var str5 = _SqliteOpr.getTableField("FAMILYTABLE" + _sProjectPrefix, "OBJECTID",
                    "NAME == '" + sFamilyName + "'");
                if (string.IsNullOrEmpty(str5))
                {
                    sColume = "NAME,PARENTID,DESCRIBE";
                    sValue = "'" + sFamilyName + "'," + str4 + ",'" + sDesc + "'";
                    str5 = _SqliteOpr.fillTable("FAMILYTABLE" + _sProjectPrefix, sColume, sValue, "");
                }
                sColume = "NAME,PARENTID,DESCRIBE";
                sValue = "'" + sName + "'," + str5 + ",'" + sDesc + "'";
                str = _SqliteOpr.fillTable("FAMILYTABLE" + _sProjectPrefix, sColume, sValue, "");
                var tListData = new List<PropertyItem>();
                TLPropertyHelper.getParameterVar(Parameters, tListData, false);
                foreach (var item in tListData)
                {
                    var str6 = "SECTION,NAME,VALUE,UNIT,TYPE,FAMILYID";
                    var str7 = "'" + item.Section + "','" + item.Name + "','" + item.Value + "','" + item.Unit + "'," +
                               item.Type + "," + str;
                    _SqliteOpr.fillTable("FMYITEMTABLE" + _sProjectPrefix, str6, str7, "");
                }
            }
            catch (Exception)
            {
            }
            return str;
        }

        public static string WriteGeoInstanceItem(GeoBlockItem geoItemt)
        {
            var str = "";
            try
            {
                var sColume = "NAME,DESCRIBE,MESHIDS";
                var sValue = "'" + geoItemt.Name + "','" + geoItemt.Desc + "','" + geoItemt.MeshIds + "'";
                str = _SqliteOpr.fillTable("GEOBLOCK" + _sProjectPrefix, sColume, sValue, "");
            }
            catch (Exception)
            {
            }
            return str;
        }

        public static string WriteGeoItem(GeoItem geoItemt)
        {
            var str = "";
            try
            {
                var sColume = "NAME,MATRIX,MESHIDS,BLOCKID,BOX,FAMILYID";
                var sValue = "'" + geoItemt.Name + "','" + geoItemt.Matrix + "','" + geoItemt.MeshIds + "'," +
                             geoItemt.BlockId + ",'" + geoItemt.Box + "'," + geoItemt.FamilyID;
                str = _SqliteOpr.fillTable("GEOTABLE" + _sProjectPrefix, sColume, sValue, "");
            }
            catch (Exception)
            {
            }
            return str;
        }

        public static int WriteMaterial(MaterialInfo LmvItem, string key)
        {
            var s = "";
            try
            {
                var str2 = "-1";
                var sFormat = "6408";
                var sMagfilter = "9729";
                var sMinfilter = "9987";
                var sWraps = "10497";
                var sWrapt = "10497";
                if ((LmvItem.Textures != null) && (LmvItem.Textures.Count > 0))
                {
                    var num = 0;
                    foreach (var info in LmvItem.Textures)
                    {
                        if ((info.TextureFilePath != null) && !string.IsNullOrEmpty(info.TextureFilePath) &&
                            File.Exists(info.TextureFilePath))
                        {
                            var textureTypeText = info.GetTextureTypeText();
                            var sName = ++num + "_" + textureTypeText;
                            var textureFilePath = info.TextureFilePath;
                            textureFilePath = textureFilePath.Substring(textureFilePath.LastIndexOf('\\') + 1);
                            textureFilePath = textureFilePath.Substring(textureFilePath.LastIndexOf('/') + 1);
                            var str11 = Guid.NewGuid().ToString();
                            var item = new TexturesItem(sName, sFormat, sMagfilter, sMinfilter, sWraps, sWrapt,
                                textureFilePath);
                            var str12 = "NAME,FORMAT,MAGFILTER,MINFILTER,WRAPS,WRAPT,IMAGENAME,TEXIMGID";
                            var str13 = "'" + item.Name + "'," + item.Format + "," + item.Magfilter + "," +
                                        item.Minfilter + "," + item.Wraps + "," + item.Wrapt + ",'" + item.Imagename +
                                        "','" + str11 + "'";
                            str2 = _SqliteOpr.fillTable("TEXTURES" + _sProjectPrefix, str12, str13, "");
                            var str14 = "OBJGUID";
                            var str15 = "'" + str11 + "'";
                            var sObjectId = _SqliteOpr.fillTable("TEXIMG" + _sProjectPrefix, str14, str15, "");
                            var bytes = Encoding.Default.GetBytes("b!@tl&kkkf&ig!le");
                            _SqliteOpr.fillTableblob("TEXIMG" + _sProjectPrefix, sObjectId, "CONTENT", bytes);
                            break;
                        }
                    }
                }
                var sAmbient = " ";
                var sEmission = " ";
                var sShininess = " ";
                var sSpecular = " ";
                var sDiffuse = " ";
                if (LmvItem.Ambient != null)
                {
                    sAmbient = string.Format("{0:F},{1:F},{2:F},1.0", LmvItem.Ambient.x, LmvItem.Ambient.y,
                        LmvItem.Ambient.z);
                }
                if (LmvItem.Emissive != null)
                {
                    sEmission = string.Format("{0:F},{1:F},{2:F},1.0", LmvItem.Emissive.x, LmvItem.Emissive.y,
                        LmvItem.Emissive.z);
                }
                sShininess = string.Format("{0:F}", LmvItem.Shininess);
                if (LmvItem.Specular != null)
                {
                    sSpecular = string.Format("{0:F},{1:F},{2:F},1.0", LmvItem.Specular.x, LmvItem.Specular.y,
                        LmvItem.Specular.z);
                }
                if (LmvItem.Color != null)
                {
                    sDiffuse = string.Format("{0:F},{1:F},{2:F},1.0", LmvItem.Color.x, LmvItem.Color.y, LmvItem.Color.z);
                }
                var item2 = new MaterialItem(key, " ", sAmbient, sEmission, sShininess, sSpecular, sDiffuse,
                    str2);
                var sColume = "NAME,AMBIENT,EMISSION,SHININESS,SPECULAR,DIFFUSE,TEXTURE_1";
                var sValue = "'" + item2.Name + "','" + item2.Ambient + "','" + item2.Emission + "','" +
                             item2.Shininess + "','" + item2.Specular + "','" + item2.Diffuse + "'," + item2.Texture1;
                s = _SqliteOpr.fillTable("MATERIALTABLE" + _sProjectPrefix, sColume, sValue, "");
            }
            catch (Exception)
            {
            }
            return int.Parse(s);
        }
    }
}