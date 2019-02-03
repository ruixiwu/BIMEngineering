using System;
using System.IO;
using BIM.Lmv.Common.TypeArray;
using BIM.Lmv.Content.Geometry.Types;

namespace BIM.Lmv.Common.Pack
{
    internal class PackFileStreamWriter : IDisposable
    {
        public PackFileStreamWriter(Stream memoryStream)
        {
            stream = new PackFileStream(memoryStream);
        }

        public PackFileStream stream { get; private set; }

        public void Dispose()
        {
            if (stream != null)
            {
                stream = null;
            }
        }

        public Box3F ReadBox3F()
        {
            var min = ReadVector3F();
            return new Box3F(min, ReadVector3F());
        }

        public void ReadMatrix3F(Matrix4F m)
        {
            var matrixf = m;
            matrixf.identity();
            for (var i = 0; i < 3; i++)
            {
                for (var j = 0; j < 3; j++)
                {
                    matrixf.elements[4*i + j] = stream.getFloat32();
                }
            }
        }

        public Vector4F ReadQuaternionF()
        {
            var x = stream.getFloat32();
            var y = stream.getFloat32();
            var z = stream.getFloat32();
            return new Vector4F(x, y, z, stream.getFloat32());
        }

        public string readString()
        {
            var len = readU32V();
            return stream.getString(len);
        }

        public Matrix4F readTransform(int entityIndex = -1, Float32Array buffer = null, int offset = 0,
            Vector3F globalOffset = null)
        {
            Vector4F vectorf2;
            Vector3F vectorf3;
            var scale = new Vector3F(1f, 1f, 1f);
            var m = new Matrix4F();
            switch (stream.getUint8())
            {
                case 0:
                    vectorf3 = (Vector3F) ReadVector3D(globalOffset);
                    m.makeTranslation(vectorf3.x, vectorf3.y, vectorf3.z);
                    break;

                case 1:
                    vectorf2 = ReadQuaternionF();
                    vectorf3 = (Vector3F) ReadVector3D(globalOffset);
                    scale.x = 1f;
                    scale.y = 1f;
                    scale.z = 1f;
                    m.compose(vectorf3, vectorf2, scale);
                    break;

                case 2:
                {
                    var num2 = stream.getFloat32();
                    vectorf2 = ReadQuaternionF();
                    vectorf3 = (Vector3F) ReadVector3D(globalOffset);
                    scale.x = num2;
                    scale.y = num2;
                    scale.z = num2;
                    m.compose(vectorf3, vectorf2, scale);
                    break;
                }
                case 3:
                    ReadMatrix3F(m);
                    vectorf3 = (Vector3F) ReadVector3D(globalOffset);
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
                    var elements = m.elements;
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

        public ushort ReadU16()
        {
            return stream.getUint16();
        }

        public int readU32V()
        {
            return ReadVarint();
        }

        public byte readU8()
        {
            return stream.getUint8();
        }

        public int ReadVarint()
        {
            return stream.GetVarints();
        }

        public Vector3D ReadVector3D(Vector3D globalOffset)
        {
            var x = stream.getFloat64();
            var y = stream.getFloat64();
            var z = stream.getFloat64();
            var vectord = new Vector3D(x, y, z);
            if (globalOffset != null)
            {
                vectord.x -= globalOffset.x;
                vectord.y -= globalOffset.y;
                vectord.z -= globalOffset.z;
            }
            return vectord;
        }

        public Vector3D ReadVector3D(Vector3F globalOffset)
        {
            return ReadVector3D((Vector3D) globalOffset);
        }

        public Vector3F ReadVector3F()
        {
            var x = stream.getFloat32();
            var y = stream.getFloat32();
            return new Vector3F(x, y, stream.getFloat32());
        }

        public void WriteBox3F(Box3F value)
        {
            WriteVector3F(value.min);
            WriteVector3F(value.max);
        }

        public void WriteMatrix3F(Matrix4F value)
        {
            for (var i = 0; i < 3; i++)
            {
                for (var j = 0; j < 3; j++)
                {
                    stream.Write(value.elements[4*i + j]);
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
                strArray = pathId.Split('/');
                length = (uint) strArray.Length;
            }
            WriteU32V(length);
            if ((strArray != null) && (strArray.Length > 0))
            {
                foreach (var str in strArray)
                {
                    var n = uint.Parse(str);
                    WriteU32V(n);
                }
            }
        }

        public void WriteQuaternionF(Vector4F value)
        {
            stream.Write(value.x);
            stream.Write(value.y);
            stream.Write(value.z);
            stream.Write(value.w);
        }

        public void WriteString(string s)
        {
            WriteU32V((uint) s.Length);
            stream.WriteStringWithoutLength(s);
        }

        public void WriteTransformAffineMatrix(Matrix4F m, Vector3D t)
        {
            stream.Write((byte) 3);
            WriteMatrix3F(m);
            WriteVector3D(t);
        }

        public void WriteTransformIdentity()
        {
            stream.Write((byte) 4);
        }

        public void WriteTransformRotationTranslation(Vector4F vector)
        {
            stream.Write((byte) 1);
            WriteQuaternionF(vector);
        }

        public void WriteTransformTranslation(Vector3D vector)
        {
            stream.Write((byte) 0);
            WriteVector3D(vector);
        }

        public void WriteTransformUniformScaleRotationTranslation(float scale, Vector4F q, Vector3D t)
        {
            stream.Write((byte) 2);
            stream.Write(scale);
            WriteQuaternionF(q);
            WriteVector3D(t);
        }

        public void WriteU16(ushort n)
        {
            stream.Write(n);
        }

        public void WriteU32V(uint n)
        {
            WriteVarint(n);
        }

        public void WriteU8(byte n)
        {
            stream.Write(n);
        }

        public void WriteVarint(uint n)
        {
            stream.WriteVarints(n);
        }

        public void WriteVector3D(Vector3D value)
        {
            stream.Write(value.x);
            stream.Write(value.y);
            stream.Write(value.z);
        }

        public void WriteVector3F(Vector3F value)
        {
            stream.Write(value.x);
            stream.Write(value.y);
            stream.Write(value.z);
        }
    }
}