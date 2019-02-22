namespace BIM.Lmv.Revit.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Text;

    internal class BFileHelp
    {
        private int _Capacity;
        private Encoding _Encoding = Encoding.UTF8;
        private MemoryStream _FileStream;
        public List<AccessorItem> _ListData = new List<AccessorItem>();
        private int _nSequence = 1;
        private BinaryReader _Reader;
        private BinaryWriter _Writer;

        public BFileHelp(int nCapacity)
        {
            this._Capacity = nCapacity;
            this._FileStream = new MemoryStream(this._Capacity);
            this._Writer = new BinaryWriter(this._FileStream, this._Encoding);
            this._Reader = new BinaryReader(this._FileStream, this._Encoding);
            this._Name = this._nSequence.ToString() + "B";
            this._Timet = DateTime.Now.ToString();
        }

        public void addData(TLGeometryBuffer ptBuffer)
        {
            if ((ptBuffer.triangleCount >= 1) && (ptBuffer.vertexCount >= 3))
            {
                this._ListData.Clear();
                int num = this.getLength();
                byte[] dst = new byte[ptBuffer.triangleCount * 6];
                Buffer.BlockCopy(ptBuffer.indices, 0, dst, 0, ptBuffer.triangleCount * 6);
                this._Writer.Write(dst, 0, ptBuffer.triangleCount * 6);
                dst = null;
                int vertexCount = ptBuffer.triangleCount * 3;
                AccessorItem item = new AccessorItem("Index", "SCALAR", num.ToString(), "0", "5123", vertexCount.ToString(), TableHelp._sCurBFileId);
                this._ListData.Add(item);
                num += ptBuffer.triangleCount * 6;
                byte[] buffer2 = new byte[ptBuffer.vertexCount * 12];
                Buffer.BlockCopy(ptBuffer.vertex, 0, buffer2, 0, ptBuffer.vertexCount * 12);
                this._Writer.Write(buffer2, 0, ptBuffer.vertexCount * 12);
                vertexCount = ptBuffer.vertexCount;
                AccessorItem item2 = new AccessorItem("Vertex", "VEC3", num.ToString(), "12", "5126", vertexCount.ToString(), TableHelp._sCurBFileId);
                this._ListData.Add(item2);
                num += ptBuffer.vertexCount * 12;
                Buffer.BlockCopy(ptBuffer.normals, 0, buffer2, 0, ptBuffer.vertexCount * 12);
                this._Writer.Write(buffer2, 0, ptBuffer.vertexCount * 12);
                AccessorItem item3 = new AccessorItem("Normal", "VEC3", num.ToString(), "12", "5126", vertexCount.ToString(), TableHelp._sCurBFileId);
                this._ListData.Add(item3);
                num += ptBuffer.vertexCount * 12;
                Buffer.BlockCopy(ptBuffer.uvs, 0, buffer2, 0, ptBuffer.vertexCount * 8);
                this._Writer.Write(buffer2, 0, ptBuffer.vertexCount * 8);
                buffer2 = null;
                AccessorItem item4 = new AccessorItem("UV", "VEC2", num.ToString(), "8", "5126", vertexCount.ToString(), TableHelp._sCurBFileId);
                this._ListData.Add(item4);
                this.writeMesh_Acces();
            }
        }

        public byte[] getBinary() => 
            this._FileStream.ToArray();

        public int getLength() => 
            ((int) this._FileStream.Length);

        public void retStart()
        {
            this.writeBFileTable();
            TableHelp._sCurBFileId = Guid.NewGuid().ToString();
            this._ListData.Clear();
            this._FileStream.Close();
            this._FileStream.Dispose();
            this._FileStream = new MemoryStream(this._Capacity);
            this._Writer = new BinaryWriter(this._FileStream, this._Encoding);
            this._Reader = new BinaryReader(this._FileStream, this._Encoding);
            this._nSequence++;
            this._Name = this._nSequence.ToString() + "B";
            this._Timet = DateTime.Now.ToString();
        }

        public void writeBFileTable()
        {
            try
            {
                string sColume = "OBJGUID";
                string sValue = "'" + TableHelp._sCurBFileId + "'";
                string sObjectId = TableHelp._SqliteOpr.fillTable("BFILETABLE" + TableHelp._sProjectPrefix, sColume, sValue, "");
                TableHelp._SqliteOpr.fillTableblob("BFILETABLE" + TableHelp._sProjectPrefix, sObjectId, "CONTENT", this._FileStream.ToArray());
            }
            catch (Exception)
            {
            }
        }

        public void writeMesh_Acces()
        {
            try
            {
                string str = " ";
                string str2 = " ";
                string str3 = " ";
                string str4 = " ";
                foreach (AccessorItem item in this._ListData)
                {
                    string str5 = "TYPE,BYTEOFFSET,BYTESTRIDE,COMPONENTTYPE,NCOUNT,BFILEID";
                    string str6 = "'" + item.Type + "'," + item.ByteOffSet + "," + item.ByteStride + "," + item.ComponentType + "," + item.Count + ",'" + item.BFileId + "'";
                    string str7 = TableHelp._SqliteOpr.fillTable("ACCESSORTABLE" + TableHelp._sProjectPrefix, str5, str6, "");
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
                this._ListData.Clear();
                MeshItem item2 = new MeshItem("mesh_" + MeshItem._nCurMesh, TLGeometryHelper._CurMaterialId.ToString(), str2, str, str3, str4);
                string sColume = "NAME,MATERIALID,ACCESSOR_POSTION,ACCESSOR_INDEX,ACCESSOR_NORMAL,ACCESSOR_TEXCOORD_0";
                string sValue = "'" + item2.Name + "'," + item2.MaterialId + "," + item2.Acc_Positon + "," + item2.Acc_Index + "," + item2.Acc_Normal + "," + item2.Acc_Texcoord;
                string str11 = TableHelp._SqliteOpr.fillTable("MESHTABLE" + TableHelp._sProjectPrefix, sColume, sValue, "");
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

        public string _Name { get; set; }

        public string _Timet { get; set; }
    }
}

