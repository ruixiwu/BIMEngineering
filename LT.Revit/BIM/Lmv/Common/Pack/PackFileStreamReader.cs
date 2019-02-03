using System;
using System.Collections.Generic;
using System.IO;
using BIM.Lmv.Common.TypeArray;
using BIM.Lmv.Content.Geometry.Types;

namespace BIM.Lmv.Common.Pack
{
    internal class PackFileStreamReader : IDisposable
    {
        private readonly List<uint> _EntryOffsets;
        public readonly List<PackEntryType> entryTypes;

        public PackFileStreamReader(Stream memoryStream)
        {
            int num4;
            entryTypes = new List<PackEntryType>();
            _EntryOffsets = new List<uint>();
            stream = new PackFileStream(memoryStream);
            type = stream.getString();
            version = stream.getInt32();
            stream.seek(stream.ByteLength - 8L);
            var num = stream.getUInt32();
            var num2 = stream.getUInt32();
            stream.seek(num2);
            var num3 = readU32V();
            for (num4 = 0; num4 < num3; num4++)
            {
                var item = PackEntryType.Read(this, num4);
                entryTypes.Add(item);
            }
            stream.seek(num);
            entryCount = readU32V();
            for (num4 = 0; num4 < entryCount; num4++)
            {
                _EntryOffsets.Add(stream.getUInt32());
            }
            stream.seek(0L);
        }

        public int entryCount { get; }

        public PackFileStream stream { get; private set; }

        public string type { get; private set; }

        public int version { get; }

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

        public string ReadPathId()
        {
            string str;
            int num2;
            if (version < 2)
            {
                var num = stream.getUint16();
                if (num == 0)
                {
                    return null;
                }
                stream.getUint16();
                if (num == 1)
                {
                    return "";
                }
                str = stream.getUint16().ToString();
                for (num2 = 2; num2 < num; num2++)
                {
                    str = str + "/" + stream.getUint16();
                }
                return str;
            }
            var num3 = readU32V();
            if (num3 == 0)
            {
                return null;
            }
            var num4 = readU32V();
            if (num3 == 1)
            {
                return num4.ToString();
            }
            str = num4 + "/" + readU32V();
            for (num2 = 2; num2 < num3; num2++)
            {
                str = str + "/" + readU32V();
            }
            return str;
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

        public PackEntryType seekToEntry(int entryIndex)
        {
            if (entryIndex >= entryCount)
            {
                return null;
            }
            stream.seek(_EntryOffsets[entryIndex]);
            var num = stream.getUInt32();
            if (num >= entryTypes.Count)
            {
                return null;
            }
            return entryTypes[(int) num];
        }
    }
}