namespace BIM.Lmv.Common.Pack
{
    using BIM.Lmv.Common.TypeArray;
    using BIM.Lmv.Content.Geometry.Types;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    internal class PackFileStreamReader : IDisposable
    {
        private readonly List<uint> _EntryOffsets;
        public readonly List<PackEntryType> entryTypes;

        public PackFileStreamReader(Stream memoryStream)
        {
            int num4;
            this.entryTypes = new List<PackEntryType>();
            this._EntryOffsets = new List<uint>();
            this.stream = new PackFileStream(memoryStream);
            this.type = this.stream.getString();
            this.version = this.stream.getInt32();
            this.stream.seek(this.stream.ByteLength - 8L);
            uint num = this.stream.getUInt32();
            uint num2 = this.stream.getUInt32();
            this.stream.seek((long) num2);
            int num3 = this.readU32V();
            for (num4 = 0; num4 < num3; num4++)
            {
                PackEntryType item = PackEntryType.Read(this, num4);
                this.entryTypes.Add(item);
            }
            this.stream.seek((long) num);
            this.entryCount = this.readU32V();
            for (num4 = 0; num4 < this.entryCount; num4++)
            {
                this._EntryOffsets.Add(this.stream.getUInt32());
            }
            this.stream.seek(0L);
        }

        public void Dispose()
        {
            if (this.stream != null)
            {
                this.stream = null;
            }
        }

        public Box3F ReadBox3F()
        {
            Vector3F min = this.ReadVector3F();
            return new Box3F(min, this.ReadVector3F());
        }

        public void ReadMatrix3F(Matrix4F m)
        {
            Matrix4F matrixf = m;
            matrixf.identity();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    matrixf.elements[(4 * i) + j] = this.stream.getFloat32();
                }
            }
        }

        public string ReadPathId()
        {
            string str;
            int num2;
            if (this.version < 2)
            {
                ushort num = this.stream.getUint16();
                if (num == 0)
                {
                    return null;
                }
                this.stream.getUint16();
                if (num == 1)
                {
                    return "";
                }
                str =this.stream.getUint16().ToString();
                for (num2 = 2; num2 < num; num2++)
                {
                    str = str + "/" + this.stream.getUint16();
                }
                return str;
            }
            int num3 = this.readU32V();
            if (num3 == 0)
            {
                return null;
            }
            int num4 = this.readU32V();
            if (num3 == 1)
            {
                return (num4.ToString());
            }
            str = num4 + "/" + this.readU32V();
            for (num2 = 2; num2 < num3; num2++)
            {
                str = str + "/" + this.readU32V();
            }
            return str;
        }

        public Vector4F ReadQuaternionF()
        {
            float x = this.stream.getFloat32();
            float y = this.stream.getFloat32();
            float z = this.stream.getFloat32();
            return new Vector4F(x, y, z, this.stream.getFloat32());
        }

        public string readString()
        {
            int len = this.readU32V();
            return this.stream.getString(len);
        }

        public Matrix4F readTransform(int entityIndex = -1, Float32Array buffer = null, int offset = 0, Vector3F globalOffset = null)
        {
            Vector4F vectorf2;
            Vector3F vectorf3;
            Vector3F scale = new Vector3F(1f, 1f, 1f);
            Matrix4F m = new Matrix4F();
            switch (this.stream.getUint8())
            {
                case 0:
                    vectorf3 = (Vector3F) this.ReadVector3D(globalOffset);
                    m.makeTranslation(vectorf3.x, vectorf3.y, vectorf3.z);
                    break;

                case 1:
                    vectorf2 = this.ReadQuaternionF();
                    vectorf3 = (Vector3F) this.ReadVector3D(globalOffset);
                    scale.x = 1f;
                    scale.y = 1f;
                    scale.z = 1f;
                    m.compose(vectorf3, vectorf2, scale);
                    break;

                case 2:
                {
                    float num2 = this.stream.getFloat32();
                    vectorf2 = this.ReadQuaternionF();
                    vectorf3 = (Vector3F) this.ReadVector3D(globalOffset);
                    scale.x = num2;
                    scale.y = num2;
                    scale.z = num2;
                    m.compose(vectorf3, vectorf2, scale);
                    break;
                }
                case 3:
                    this.ReadMatrix3F(m);
                    vectorf3 = (Vector3F) this.ReadVector3D(globalOffset);
                    m.setPosition(vectorf3);
                    break;

                case 4:
                    m.identity();
                    if (globalOffset != null)
                    {
                        m.elements[12] -= globalOffset.x;
                        m.elements[13] -= globalOffset.y;
                        m.elements[14] -= globalOffset.z;
                    }
                    break;

                default:
                    throw new InvalidOperationException();
            }
            if (entityIndex > -1)
            {
                if (buffer != null)
                {
                    float[] elements = m.elements;
                    buffer[offset] = elements[0];
                    buffer[offset + 1] = elements[1];
                    buffer[offset + 2] = elements[2];
                    buffer[offset + 3] = elements[4];
                    buffer[offset + 4] = elements[5];
                    buffer[offset + 5] = elements[6];
                    buffer[offset + 6] = elements[8];
                    buffer[offset + 7] = elements[9];
                    buffer[offset + 8] = elements[10];
                    buffer[offset + 9] = elements[12];
                    buffer[offset + 10] = elements[13];
                    buffer[offset + 11] = elements[14];
                }
                return null;
            }
            return m;
        }

        public ushort ReadU16() => 
            this.stream.getUint16();

        public int readU32V() => 
            this.ReadVarint();

        public byte readU8() => 
            this.stream.getUint8();

        public int ReadVarint() => 
            this.stream.GetVarints();

        public Vector3D ReadVector3D(Vector3D globalOffset)
        {
            double x = this.stream.getFloat64();
            double y = this.stream.getFloat64();
            double z = this.stream.getFloat64();
            Vector3D vectord = new Vector3D(x, y, z);
            if (globalOffset != null)
            {
                vectord.x -= globalOffset.x;
                vectord.y -= globalOffset.y;
                vectord.z -= globalOffset.z;
            }
            return vectord;
        }

        public Vector3D ReadVector3D(Vector3F globalOffset) => 
            this.ReadVector3D((Vector3D) globalOffset);

        public Vector3F ReadVector3F()
        {
            float x = this.stream.getFloat32();
            float y = this.stream.getFloat32();
            return new Vector3F(x, y, this.stream.getFloat32());
        }

        public PackEntryType seekToEntry(int entryIndex)
        {
            if (entryIndex >= this.entryCount)
            {
                return null;
            }
            this.stream.seek((long) ((ulong) this._EntryOffsets[entryIndex]));
            uint num = this.stream.getUInt32();
            if (num >= this.entryTypes.Count)
            {
                return null;
            }
            return this.entryTypes[(int) num];
        }

        public int entryCount { get; private set; }

        public PackFileStream stream { get; private set; }

        public string type { get; private set; }

        public int version { get; private set; }
    }
}

