namespace BIM.Lmv.Common.Pack
{
    using BIM.Lmv.Common.TypeArray;
    using BIM.Lmv.Content.Geometry.Types;
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    internal class PackFileStreamWriter : IDisposable
    {
        public PackFileStreamWriter(Stream memoryStream)
        {
            this.stream = new PackFileStream(memoryStream);
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

        public void WriteBox3F(Box3F value)
        {
            this.WriteVector3F(value.min);
            this.WriteVector3F(value.max);
        }

        public void WriteMatrix3F(Matrix4F value)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    this.stream.Write(value.elements[(4 * i) + j]);
                }
            }
        }

        public void WritePathId(string pathId)
        {
            uint length;
            string[] strArray = null;
            if (pathId == null)
            {
                length = 0;
            }
            else if (pathId == "")
            {
                length = 1;
            }
            else
            {
                strArray = pathId.Split(new char[] { '/' });
                length = (uint) strArray.Length;
            }
            this.WriteU32V(length);
            if ((strArray != null) && (strArray.Length > 0))
            {
                foreach (string str in strArray)
                {
                    uint n = uint.Parse(str);
                    this.WriteU32V(n);
                }
            }
        }

        public void WriteQuaternionF(Vector4F value)
        {
            this.stream.Write(value.x);
            this.stream.Write(value.y);
            this.stream.Write(value.z);
            this.stream.Write(value.w);
        }

        public void WriteString(string s)
        {
            this.WriteU32V((uint) s.Length);
            this.stream.WriteStringWithoutLength(s);
        }

        public void WriteTransformAffineMatrix(Matrix4F m, Vector3D t)
        {
            this.stream.Write((byte) 3);
            this.WriteMatrix3F(m);
            this.WriteVector3D(t);
        }

        public void WriteTransformIdentity()
        {
            this.stream.Write((byte) 4);
        }

        public void WriteTransformRotationTranslation(Vector4F vector)
        {
            this.stream.Write((byte) 1);
            this.WriteQuaternionF(vector);
        }

        public void WriteTransformTranslation(Vector3D vector)
        {
            this.stream.Write((byte) 0);
            this.WriteVector3D(vector);
        }

        public void WriteTransformUniformScaleRotationTranslation(float scale, Vector4F q, Vector3D t)
        {
            this.stream.Write((byte) 2);
            this.stream.Write(scale);
            this.WriteQuaternionF(q);
            this.WriteVector3D(t);
        }

        public void WriteU16(ushort n)
        {
            this.stream.Write(n);
        }

        public void WriteU32V(uint n)
        {
            this.WriteVarint(n);
        }

        public void WriteU8(byte n)
        {
            this.stream.Write(n);
        }

        public void WriteVarint(uint n)
        {
            this.stream.WriteVarints(n);
        }

        public void WriteVector3D(Vector3D value)
        {
            this.stream.Write(value.x);
            this.stream.Write(value.y);
            this.stream.Write(value.z);
        }

        public void WriteVector3F(Vector3F value)
        {
            this.stream.Write(value.x);
            this.stream.Write(value.y);
            this.stream.Write(value.z);
        }

        public PackFileStream stream { get; private set; }
    }
}

