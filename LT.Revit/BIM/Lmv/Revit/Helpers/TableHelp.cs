using System.Web.UI.WebControls;

namespace BIM.Lmv.Revit.Helpers
{
    using Autodesk.Revit.DB;
    using BIM.Lmv.Content.Geometry.Types;
    using BIM.Lmv.Types;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    using System.Web;
    using System.Windows.Forms;
    using Utils;

    internal class TableHelp
    {
        public bool _bIsInstance = false;
        public static Dictionary<string, string> _configDic = new Dictionary<string, string>();
        public static Vector3F _EleBoxMax = new Vector3F(float.MinValue, float.MinValue, float.MinValue);
        public static Vector3F _EleBoxMin = new Vector3F(float.MaxValue, float.MaxValue, float.MaxValue);
        public Dictionary<string, string> _FamilyIns;
        public int _InsNodeIdDelay = 0;
        public Dictionary<int, Vector3F> _InstanceBoxMax;
        public Dictionary<int, Vector3F> _InstanceBoxMin;
        public Dictionary<int, string> _InstanceTemplates;
        public string _sBlockId = "";
        public static string _sCurBFileId;
        public static string _sDirPath;
        public string _sMatrix = "";
        public SceneInfo _SneneInfo;
        public static string _sPathData;
        public static string _sPathData2;
        public static string _sProjectPrefix;
        public static SqliteOpr _SqliteOpr = new SqliteOpr();
        public static string _sUrlRoot;
        public static string _sZipKey;
        public static string _sZipPath = "";
        public Vector3F _TLGlobalBoxMax = new Vector3F(float.MinValue, float.MinValue, float.MinValue);
        public Vector3F _TLGlobalBoxMin = new Vector3F(float.MaxValue, float.MaxValue, float.MaxValue);
        public Autodesk.Revit.DB.Transform _TransformDelay;
        public static bool g_bFree = false;
        public static string g_modelName;
        public static string g_modelTypeNo;
        public static string g_stage;
        public static string sUser;

        public TableHelp(Document doc)
        {
            _SqliteOpr.connectToDatabase(_sPathData);
            _SqliteOpr.createTable(_sProjectPrefix);
            this._SneneInfo = ExportHelper.GetSceneInfo(doc);
            this._TransformDelay = Autodesk.Revit.DB.Transform.Identity;
            this._InstanceTemplates = new Dictionary<int, string>();
            this._InstanceBoxMin = new Dictionary<int, Vector3F>();
            this._InstanceBoxMax = new Dictionary<int, Vector3F>();
            this._FamilyIns = new Dictionary<string, string>();
            _sCurBFileId = Guid.NewGuid().ToString();
        }

        public void AddProject(string strSourceFile)
        {
            try
            {
                string sID = _sProjectPrefix.Substring(1);
                _sZipKey = Guid.NewGuid().ToString();
                string str2 = Guid.NewGuid().ToString();
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(strSourceFile);
                string str4 = g_modelName;
                string extension = Path.GetExtension(strSourceFile);
                string sUser = TableHelp.sUser;
                string str7 = g_modelTypeNo;
                string sColume = "NAME,TYPE,AFFIXFILEID";
                string sValue = "'" + fileNameWithoutExtension + "','" + extension + "','" + str2 + "'";
                string str10 = _SqliteOpr.fillTable("AFFIXTABLE" + _sProjectPrefix, sColume, sValue, "");
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
            new Thread(new ParameterizedThreadStart(TableHelp.delDirFunc)) { IsBackground = true }.Start(strFile);
        }

        public static void delDirFunc(object data)
        {
            try
            {
                string path = data as string;
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
            new Thread(new ParameterizedThreadStart(TableHelp.delFileFunc)) { IsBackground = true }.Start(strFile);
        }

        public static void delFileFunc(object data)
        {
            string path = data as string;
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
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
            }
            catch (Exception)
            {
            }
        }

        public void ElementBegin()
        {
            this.ClearEleBox();
            this._sBlockId = "";
            this._bIsInstance = false;
            TLGeometryHelper.strMeshIds = "";
        }

        public void ElementEnd(bool bHasGeometry)
        {
            if ((bHasGeometry && (_EleBoxMin != null)) && (_EleBoxMin.x != float.MaxValue))
            {
                this._TLGlobalBoxMin.x = Math.Min(this._TLGlobalBoxMin.x, _EleBoxMin.x);
                this._TLGlobalBoxMin.y = Math.Min(this._TLGlobalBoxMin.y, _EleBoxMin.y);
                this._TLGlobalBoxMin.z = Math.Min(this._TLGlobalBoxMin.z, _EleBoxMin.z);
                this._TLGlobalBoxMax.x = Math.Max(this._TLGlobalBoxMax.x, _EleBoxMax.x);
                this._TLGlobalBoxMax.y = Math.Max(this._TLGlobalBoxMax.y, _EleBoxMax.y);
                this._TLGlobalBoxMax.z = Math.Max(this._TLGlobalBoxMax.z, _EleBoxMax.z);
            }
        }

        public string ElemWriteTable(Element curElem, Document tDoc, Autodesk.Revit.DB.Transform tTransform, bool bReallyInstance)
        {
            string str = "";
            if (this._bIsInstance && bReallyInstance)
            {
                this.InstanceEndDelay();
            }
            if (!string.IsNullOrEmpty(TLGeometryHelper.strMeshIds) || !string.IsNullOrEmpty(this._sBlockId))
            {
                string str2 = "";
                ElementId typeId = curElem.GetTypeId();
                Element element = tDoc.GetElement(typeId);
                if ((typeId != null) && (element != null))
                {
                    string str3;
                    if (this._FamilyIns.TryGetValue(typeId.ToString(), out str3))
                    {
                        str2 = str3;
                    }
                    else
                    {
                        string name = curElem.Category.Name;
                        string sFamilyName = name;
                        string sName = curElem.Name;
                        ElementType type = element as ElementType;
                        if (type != null)
                        {
                            sFamilyName = type.FamilyName;
                            sName = type.Name;
                            if (type.Category != null)
                            {
                                name = type.Category.Name;
                            }
                        }
                        sName = sName.Replace('\'', '`');
                        sFamilyName = sFamilyName.Replace('\'', '`');
                        str2 = WriteFamilyTable(element.Parameters, sName, sFamilyName, name, typeId.ToString());
                        this._FamilyIns.Add(typeId.ToString(), str2);
                    }
                }
                string uniqueId = curElem.UniqueId;
                string title = tDoc.Title;
                string sBox = $"{_EleBoxMin.x:F3},{_EleBoxMin.y:F3},{_EleBoxMin.z:F3},{_EleBoxMax.x:F3},{_EleBoxMax.y:F3},{_EleBoxMax.z:F3}";
                if (this._bIsInstance && bReallyInstance)
                {
                    GeoItem geoItemt = new GeoItem(uniqueId, this._sMatrix, "null", this._sBlockId, sBox, title, str2);
                    str = WriteGeoItem(geoItemt);
                }
                else
                {
                    Matrix4F matrixFrom = TransformHelper.GetMatrixFrom(tTransform);
                    Vector3F v = new Vector3F((float) tTransform.Origin.X, (float) tTransform.Origin.Y, (float) tTransform.Origin.Z);
                    matrixFrom.setPosition(v);
                    this._sMatrix = matrixFrom.toString();
                    GeoItem item2 = new GeoItem(uniqueId, this._sMatrix, TLGeometryHelper.strMeshIds, "-1", sBox, title, str2);
                    str = WriteGeoItem(item2);
                }
                TLGeometryHelper.strMeshIds = "";
            }
            return str;
        }

        public static string exeRequest(string sUrlseg, string sMethod, string sData, ref string sRetData, bool bFirst = false)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            string str = "success";
            HttpWebRequest request = WebRequest.Create(_sUrlRoot + sUrlseg) as HttpWebRequest;
            request.Timeout = 0x493e0;
            request.Method = sMethod;
            request.ContentType = "application/json;charset=UTF-8";
            if ((sMethod == "POST") && !string.IsNullOrEmpty(sData))
            {
                byte[] bytes = Encoding.UTF8.GetBytes(sData.ToString());
                request.ContentLength = bytes.Length;
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(bytes, 0, bytes.Length);
                }
            }
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                JObject obj2 = JObject.Parse(new StreamReader(response.GetResponseStream(), Encoding.ASCII).ReadToEnd());
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
            TimeSpan elapsed = stopwatch.Elapsed;
            return str;
        }

        public void Finish()
        {
            CoordinaeSysItem geoItemt = new CoordinaeSysItem {
                Name = "geosys",
                False_easting = this._SneneInfo.Longitude.ToString(),
                False_northing = this._SneneInfo.Latitude.ToString(),
                Centralmeridian = this._SneneInfo.AngleToTrueNorth.ToString()
            };
            WriteCoordSysItem(geoItemt);
            try
            {
                if (System.IO.File.Exists(_sZipPath))
                {
                    byte[] bArray = System.IO.File.ReadAllBytes(_sZipPath);
                    string sColume = "OBJGUID";
                    string sValue = "'" + _sZipKey + "'";
                    string sObjectId = _SqliteOpr.fillTable("AFFIXFILE" + _sProjectPrefix, sColume, sValue, "");
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
                if (System.IO.File.Exists(_sPathData))
                {
                    System.IO.File.Copy(_sPathData, _sPathData2, true);
                    Thread.Sleep(0x3e8);
                    byte[] bArr = System.IO.File.ReadAllBytes(_sPathData2);
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
                string str4 = _sProjectPrefix.Substring(1);
                Dictionary<string, string> dictParam = new Dictionary<string, string> {
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
            MessageBox.Show("数据上传完成！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

        public string HandleInstanceTable(int nKey)
        {
            string sName = nKey.ToString();
            string sDesc = sName + "_Instance";
            GeoBlockItem geoItemt = new GeoBlockItem(sName, sDesc, TLGeometryHelper.strMeshIds);
            string str3 = WriteGeoInstanceItem(geoItemt);
            TLGeometryHelper.strMeshIds = "";
            return str3;
        }

        public static bool InitConfig(string exePath)
        {
            try
            {
                string str;
                if (!System.IO.File.Exists(exePath))
                {
                    return false;
                }
                FileStream stream = new FileStream(exePath, FileMode.OpenOrCreate);
                StreamReader reader = new StreamReader(stream);
                while ((str = reader.ReadLine()) != null)
                {
                    int index = str.IndexOf('=');
                    _configDic.Add(str.Substring(0, index).Trim(), str.Substring(index + 1).Trim());
                }
                reader.Close();
                stream.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool InstanceBegin(Autodesk.Revit.DB.Transform tTransform, int nSymbolID)
        {
            string str;
            this._bIsInstance = true;
            Matrix4F matrixFrom = TransformHelper.GetMatrixFrom(tTransform);
            Vector3F v = new Vector3F((float) tTransform.Origin.X, (float) tTransform.Origin.Y, (float) tTransform.Origin.Z);
            matrixFrom.setPosition(v);
            this._sMatrix = matrixFrom.toString();
            if (this._InstanceTemplates.TryGetValue(nSymbolID, out str))
            {
                this._sBlockId = str;
                return true;
            }
            return false;
        }

        public void InstanceEnd(Autodesk.Revit.DB.Transform tTransform, int nSymbolID)
        {
            this._InsNodeIdDelay = nSymbolID;
            this._TransformDelay = tTransform;
        }

        public void InstanceEndDelay()
        {
            string str;
            bool flag = false;
            if (!this._InstanceTemplates.TryGetValue(this._InsNodeIdDelay, out str))
            {
                if (!string.IsNullOrEmpty(TLGeometryHelper.strMeshIds))
                {
                    this._sBlockId = this.HandleInstanceTable(this._InsNodeIdDelay);
                    this._InstanceTemplates.Add(this._InsNodeIdDelay, this._sBlockId);
                    this._InstanceBoxMin.Add(this._InsNodeIdDelay, _EleBoxMin);
                    this._InstanceBoxMax.Add(this._InsNodeIdDelay, _EleBoxMax);
                    flag = true;
                }
            }
            else
            {
                flag = true;
            }
            if (flag)
            {
                Matrix4F matrixFrom = TransformHelper.GetMatrixFrom(this._TransformDelay);
                Vector3F v = new Vector3F((float) this._TransformDelay.Origin.X, (float) this._TransformDelay.Origin.Y, (float) this._TransformDelay.Origin.Z);
                matrixFrom.setPosition(v);
                _EleBoxMin = this._InstanceBoxMin[this._InsNodeIdDelay];
                _EleBoxMax = this._InstanceBoxMax[this._InsNodeIdDelay];
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
                foreach (PropertyItem item in ListData)
                {
                    string sColume = "SECTION,NAME,VALUE,UNIT,TYPE,GEOID";
                    string sValue = "'" + item.Section + "','" + item.Name + "','" + item.Value + "','" + item.Unit + "'," + item.Type + "," + strGeoID;
                    _SqliteOpr.fillTable("PROTABLE" + _sProjectPrefix, sColume, sValue, "");
                }
            }
            catch (Exception)
            {
            }
        }

        public static string Upload_Request(string sUrlseg, string fileNamePath, BinaryReader brIn = null, long nFileLength = 0L)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            string requestUriString = _sUrlRoot + sUrlseg;
            string str2 = "";
            string str3 = "Binaryfile";
            string str4 = "----------" + DateTime.Now.Ticks.ToString("x");
            byte[] bytes = Encoding.ASCII.GetBytes("\r\n--" + str4 + "\r\n");
            StringBuilder builder = new StringBuilder();
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
            string s = builder.ToString();
            byte[] buffer = Encoding.UTF8.GetBytes(s);
            long length = nFileLength;
            BinaryReader reader = brIn;
            FileStream input = null;
            if (length == 0L)
            {
                input = new FileStream(fileNamePath, FileMode.Open, FileAccess.Read);
                reader = new BinaryReader(input);
                length = input.Length;
            }
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(requestUriString);
            request.Method = "POST";
            request.AllowWriteStreamBuffering = false;
            request.Timeout = 0x1e8480;
            request.ContentType = "multipart/form-data; boundary=" + str4;
            long num2 = (length + buffer.Length) + bytes.Length;
            request.ContentLength = num2;
            try
            {
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(buffer, 0, buffer.Length);
                int count = Math.Min(0x7d000, (int) length);
                byte[] buffer3 = new byte[count];
                for (int i = reader.Read(buffer3, 0, count); i > 0; i = reader.Read(buffer3, 0, count))
                {
                    requestStream.Write(buffer3, 0, i);
                }
                requestStream.Write(bytes, 0, bytes.Length);
                Stream responseStream = request.GetResponse().GetResponseStream();
                StreamReader reader2 = new StreamReader(responseStream);
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
            TimeSpan elapsed = stopwatch.Elapsed;
            return str2;
        }

        public static string UploadFileXss(string sUrlseg, byte[] bArr)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            string requestUriString = _sUrlRoot + sUrlseg;
            string str2 = "";
            string str3 = "Binaryfile";
            try
            {
                HttpWebRequest request = WebRequest.Create(requestUriString) as HttpWebRequest;
                request.Timeout = 0x1e8480;
                request.Method = "POST";
                string str4 = DateTime.Now.Ticks.ToString("X");
                request.ContentType = "multipart/form-data;charset=utf-8;boundary=" + str4;
                byte[] bytes = Encoding.UTF8.GetBytes("\r\n--" + str4 + "\r\n");
                byte[] buffer = Encoding.UTF8.GetBytes("\r\n--" + str4 + "--\r\n");
                StringBuilder builder = new StringBuilder($"Content-Disposition:form-data;name=\"file\";filename=\"{str3}\"\r\nContent-Type:application/octet-stream");
                byte[] buffer3 = Encoding.UTF8.GetBytes(builder.ToString());
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Write(buffer3, 0, buffer3.Length);
                requestStream.Write(bArr, 0, bArr.Length);
                requestStream.Write(buffer, 0, buffer.Length);
                requestStream.Close();
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                str2 = new StreamReader(response.GetResponseStream(), Encoding.UTF8).ReadToEnd();
            }
            catch (Exception exception)
            {
                str2 = exception.ToString();
            }
            stopwatch.Stop();
            TimeSpan elapsed = stopwatch.Elapsed;
            return str2;
        }

        public static string WriteCoordSysItem(CoordinaeSysItem geoItemt)
        {
            string str = "";
            try
            {
                string sColume = "NAME,FALSE_EASTING,FALSE_NORTHING,CENTRALMERIDIAN";
                string sValue = "'" + geoItemt.Name + "'," + geoItemt.False_easting + "," + geoItemt.False_northing + "," + geoItemt.Centralmeridian;
                str = _SqliteOpr.fillTable("COORDINATE_SYSTEM", sColume, sValue, "");
            }
            catch (Exception)
            {
            }
            return str;
        }

        public static string WriteFamilyTable(ParameterSet Parameters, string sName, string sFamilyName, string sCategory, string sDesc)
        {
            string str = "";
            string sColume = " ";
            string sValue = " ";
            try
            {
                string str4 = _SqliteOpr.getTableField("FAMILYTABLE" + _sProjectPrefix, "OBJECTID", "NAME == '" + sCategory + "'");
                if (string.IsNullOrEmpty(str4))
                {
                    sColume = "NAME,PARENTID,DESCRIBE";
                    sValue = "'" + sCategory + "',-1,'" + sDesc + "'";
                    str4 = _SqliteOpr.fillTable("FAMILYTABLE" + _sProjectPrefix, sColume, sValue, "");
                }
                string str5 = _SqliteOpr.getTableField("FAMILYTABLE" + _sProjectPrefix, "OBJECTID", "NAME == '" + sFamilyName + "'");
                if (string.IsNullOrEmpty(str5))
                {
                    sColume = "NAME,PARENTID,DESCRIBE";
                    sValue = "'" + sFamilyName + "'," + str4 + ",'" + sDesc + "'";
                    str5 = _SqliteOpr.fillTable("FAMILYTABLE" + _sProjectPrefix, sColume, sValue, "");
                }
                sColume = "NAME,PARENTID,DESCRIBE";
                sValue = "'" + sName + "'," + str5 + ",'" + sDesc + "'";
                str = _SqliteOpr.fillTable("FAMILYTABLE" + _sProjectPrefix, sColume, sValue, "");
                List<PropertyItem> tListData = new List<PropertyItem>();
                TLPropertyHelper.getParameterVar(Parameters, tListData, false);
                foreach (PropertyItem item in tListData)
                {
                    string str6 = "SECTION,NAME,VALUE,UNIT,TYPE,FAMILYID";
                    string str7 = "'" + item.Section + "','" + item.Name + "','" + item.Value + "','" + item.Unit + "'," + item.Type + "," + str;
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
            string str = "";
            try
            {
                string sColume = "NAME,DESCRIBE,MESHIDS";
                string sValue = "'" + geoItemt.Name + "','" + geoItemt.Desc + "','" + geoItemt.MeshIds + "'";
                str = _SqliteOpr.fillTable("GEOBLOCK" + _sProjectPrefix, sColume, sValue, "");
            }
            catch (Exception)
            {
            }
            return str;
        }

        public static string WriteGeoItem(GeoItem geoItemt)
        {
            string str = "";
            try
            {
                string sColume = "NAME,MATRIX,MESHIDS,BLOCKID,BOX,FAMILYID";
                string sValue = "'" + geoItemt.Name + "','" + geoItemt.Matrix + "','" + geoItemt.MeshIds + "'," + geoItemt.BlockId + ",'" + geoItemt.Box + "'," + geoItemt.FamilyID;
                str = _SqliteOpr.fillTable("GEOTABLE" + _sProjectPrefix, sColume, sValue, "");
            }
            catch (Exception)
            {
            }
            return str;
        }

        public static int WriteMaterial(MaterialInfo LmvItem, string key)
        {
            string s = "";
            try
            {
                string str2 = "-1";
                string sFormat = "6408";
                string sMagfilter = "9729";
                string sMinfilter = "9987";
                string sWraps = "10497";
                string sWrapt = "10497";
                if ((LmvItem.Textures != null) && (LmvItem.Textures.Count > 0))
                {
                    int num = 0;
                    foreach (TextureInfo info in LmvItem.Textures)
                    {
                        if (((info.TextureFilePath != null) && !string.IsNullOrEmpty(info.TextureFilePath)) && System.IO.File.Exists(info.TextureFilePath))
                        {
                            string textureTypeText = info.GetTextureTypeText();
                            string sName = ++num + "_" + textureTypeText;
                            string textureFilePath = info.TextureFilePath;
                            textureFilePath = textureFilePath.Substring(textureFilePath.LastIndexOf('\\') + 1);
                            textureFilePath = textureFilePath.Substring(textureFilePath.LastIndexOf('/') + 1);
                            string str11 = Guid.NewGuid().ToString();
                            TexturesItem item = new TexturesItem(sName, sFormat, sMagfilter, sMinfilter, sWraps, sWrapt, textureFilePath);
                            string str12 = "NAME,FORMAT,MAGFILTER,MINFILTER,WRAPS,WRAPT,IMAGENAME,TEXIMGID";
                            string str13 = "'" + item.Name + "'," + item.Format + "," + item.Magfilter + "," + item.Minfilter + "," + item.Wraps + "," + item.Wrapt + ",'" + item.Imagename + "','" + str11 + "'";
                            str2 = _SqliteOpr.fillTable("TEXTURES" + _sProjectPrefix, str12, str13, "");
                            byte[] bArray = System.IO.File.ReadAllBytes(info.TextureFilePath);
                            string str14 = "OBJGUID";
                            string str15 = "'" + str11 + "'";
                            string sObjectId = _SqliteOpr.fillTable("TEXIMG" + _sProjectPrefix, str14, str15, "");
                            _SqliteOpr.fillTableblob("TEXIMG" + _sProjectPrefix, sObjectId, "CONTENT", bArray);
                            break;
                        }
                    }
                }
                string sAmbient = " ";
                string sEmission = " ";
                string sShininess = " ";
                string sSpecular = " ";
                string sDiffuse = " ";
                if (LmvItem.Ambient != null)
                {
                    sAmbient = $"{LmvItem.Ambient.x:F},{LmvItem.Ambient.y:F},{LmvItem.Ambient.z:F},1.0";
                }
                if (LmvItem.Emissive != null)
                {
                    sEmission = $"{LmvItem.Emissive.x:F},{LmvItem.Emissive.y:F},{LmvItem.Emissive.z:F},1.0";
                }
                sShininess = $"{LmvItem.Shininess:F}";
                if (LmvItem.Specular != null)
                {
                    sSpecular = $"{LmvItem.Specular.x:F},{LmvItem.Specular.y:F},{LmvItem.Specular.z:F},1.0";
                }
                if (LmvItem.Color != null)
                {
                    sDiffuse = $"{LmvItem.Color.x:F},{LmvItem.Color.y:F},{LmvItem.Color.z:F},1.0";
                }
                MaterialItem item2 = new MaterialItem(key, " ", sAmbient, sEmission, sShininess, sSpecular, sDiffuse, str2);
                string sColume = "NAME,AMBIENT,EMISSION,SHININESS,SPECULAR,DIFFUSE,TEXTURE_1";
                string sValue = "'" + item2.Name + "','" + item2.Ambient + "','" + item2.Emission + "','" + item2.Shininess + "','" + item2.Specular + "','" + item2.Diffuse + "'," + item2.Texture1;
                s = _SqliteOpr.fillTable("MATERIALTABLE" + _sProjectPrefix, sColume, sValue, "");
            }
            catch (Exception)
            {
            }
            return int.Parse(s);
        }
    }
}

