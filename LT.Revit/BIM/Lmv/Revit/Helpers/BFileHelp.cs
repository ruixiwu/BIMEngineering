using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BIM.Lmv.Revit.Helpers
{
    internal class BFileHelp
    {
        private readonly int _Capacity;
        private readonly Encoding _Encoding = Encoding.UTF8;
        private MemoryStream _FileStream;
        public List<AccessorItem> _ListData = new List<AccessorItem>();
        private int _nSequence = 1;
        private BinaryReader _Reader;
        private BinaryWriter _Writer;

        public BFileHelp(int nCapacity)
        {
            _Capacity = nCapacity;
            _FileStream = new MemoryStream(_Capacity);
            _Writer = new BinaryWriter(_FileStream, _Encoding);
            _Reader = new BinaryReader(_FileStream, _Encoding);
            _Name = _nSequence + "B";
            _Timet = DateTime.Now.ToString();
        }

        public string _Name { get; set; }

        public string _Timet { get; set; }

        public void addData(TLGeometryBuffer ptBuffer)
        {
            if ((ptBuffer.triangleCount >= 1) && (ptBuffer.vertexCount >= 3))
            {
                _ListData.Clear();
                var num = getLength();
                var dst = new byte[ptBuffer.triangleCount*6];
                Buffer.BlockCopy(ptBuffer.indices, 0, dst, 0, ptBuffer.triangleCount*6);
                _Writer.Write(dst, 0, ptBuffer.triangleCount*6);
                dst = null;
                var vertexCount = ptBuffer.triangleCount*3;
                var item = new AccessorItem("Index", "SCALAR", num.ToString(), "0", "5123",
                    vertexCount.ToString(), TableHelp._sCurBFileId);
                _ListData.Add(item);
                num += ptBuffer.triangleCount*6;
                var buffer2 = new byte[ptBuffer.vertexCount*12];
                Buffer.BlockCopy(ptBuffer.vertex, 0, buffer2, 0, ptBuffer.vertexCount*12);
                _Writer.Write(buffer2, 0, ptBuffer.vertexCount*12);
                vertexCount = ptBuffer.vertexCount;
                var item2 = new AccessorItem("Vertex", "VEC3", num.ToString(), "12", "5126",
                    vertexCount.ToString(), TableHelp._sCurBFileId);
                _ListData.Add(item2);
                num += ptBuffer.vertexCount*12;
                Buffer.BlockCopy(ptBuffer.normals, 0, buffer2, 0, ptBuffer.vertexCount*12);
                _Writer.Write(buffer2, 0, ptBuffer.vertexCount*12);
                var item3 = new AccessorItem("Normal", "VEC3", num.ToString(), "12", "5126",
                    vertexCount.ToString(), TableHelp._sCurBFileId);
                _ListData.Add(item3);
                num += ptBuffer.vertexCount*12;
                Buffer.BlockCopy(ptBuffer.uvs, 0, buffer2, 0, ptBuffer.vertexCount*8);
                _Writer.Write(buffer2, 0, ptBuffer.vertexCount*8);
                buffer2 = null;
                var item4 = new AccessorItem("UV", "VEC2", num.ToString(), "8", "5126", vertexCount.ToString(),
                    TableHelp._sCurBFileId);
                _ListData.Add(item4);
                writeMesh_Acces();
            }
        }

        public byte[] getBinary()
        {
            return _FileStream.ToArray();
        }

        public int getLength()
        {
            return (int) _FileStream.Length;
        }

        public void retStart()
        {
            writeBFileTable();
            TableHelp._sCurBFileId = Guid.NewGuid().ToString();
            _ListData.Clear();
            _FileStream.Close();
            _FileStream.Dispose();
            _FileStream = new MemoryStream(_Capacity);
            _Writer = new BinaryWriter(_FileStream, _Encoding);
            _Reader = new BinaryReader(_FileStream, _Encoding);
            _nSequence++;
            _Name = _nSequence + "B";
            _Timet = DateTime.Now.ToString();
        }

        public void writeBFileTable()
        {
            try
            {
                var sColume = "OBJGUID";
                var sValue = "'" + TableHelp._sCurBFileId + "'";
                var sObjectId = TableHelp._SqliteOpr.fillTable("BFILETABLE" + TableHelp._sProjectPrefix, sColume,
                    sValue, "");
                var bytes = Encoding.Default.GetBytes(TLGeometryHelper.strMeshIds + "b@tl&f&igle");
                TableHelp._SqliteOpr.fillTableblob("BFILETABLE" + TableHelp._sProjectPrefix, sObjectId, "CONTENT", bytes);
            }
            catch (Exception)
            {
            }
        }

        public void writeMesh_Acces()
        {
            try
            {
                var str = " ";
                var str2 = " ";
                var str3 = " ";
                var str4 = " ";
                foreach (var item in _ListData)
                {
                    var str5 = "TYPE,BYTEOFFSET,BYTESTRIDE,COMPONENTTYPE,NCOUNT,BFILEID";
                    var str6 = "'" + item.Type + "'," + item.ByteOffSet + "," + item.ByteStride + "," +
                               item.ComponentType + "," + item.Count + ",'" + item.BFileId + "'";
                    var str7 = TableHelp._SqliteOpr.fillTable("ACCESSORTABLE" + TableHelp._sProjectPrefix, str5, str6,
                        "");
                    if (item.AccType == "Index")
                    {
                        str = str7;
                    }
                    else if (item.AccType == "Vertex")
                    {
                        str2 = str7;
                    }
                    else if (item.AccType == "Normal")
                    {
                        str3 = str7;
                    }
                    else if (item.AccType == "UV")
                    {
                        str4 = str7;
                    }
                }
                _ListData.Clear();
                var item2 = new MeshItem("mesh_" + MeshItem._nCurMesh, TLGeometryHelper._CurMaterialId.ToString(),
                    str2, str, str3, str4);
                var sColume = "NAME,MATERIALID,ACCESSOR_POSTION,ACCESSOR_INDEX,ACCESSOR_NORMAL,ACCESSOR_TEXCOORD_0";
                var sValue = "'" + item2.Name + "'," + item2.MaterialId + "," + item2.Acc_Positon + "," +
                             item2.Acc_Index + "," + item2.Acc_Normal + "," + item2.Acc_Texcoord;
                var str11 = TableHelp._SqliteOpr.fillTable("MESHTABLE" + TableHelp._sProjectPrefix, sColume, sValue,
                    "");
                if (string.IsNullOrEmpty(TLGeometryHelper.strMeshIds))
                {
                    TLGeometryHelper.strMeshIds = str11;
                }
                else
                {
                    TLGeometryHelper.strMeshIds = TLGeometryHelper.strMeshIds + "," + str11;
                }
            }
            catch (Exception)
            {
            }
        }
    }
}