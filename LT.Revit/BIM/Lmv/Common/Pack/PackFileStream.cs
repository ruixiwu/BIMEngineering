using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace BIM.Lmv.Common.Pack
{
    internal class PackFileStream
    {
        public static bool DebugMode = false;
        private readonly Encoding _Encoding = Encoding.UTF8;
        private readonly BinaryReader _Reader;
        private readonly Stream _Stream;
        private readonly BinaryWriter _Writer;

        public PackFileStream(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            _Stream = stream;
            if (!stream.CanSeek)
            {
                throw new ArgumentException("Sstream can't seek!");
            }
            if (stream.CanWrite)
            {
                _Writer = new BinaryWriter(stream, _Encoding);
            }
            if (stream.CanRead)
            {
                _Reader = new BinaryReader(stream, _Encoding);
            }
        }

        public long ByteLength
        {
            get { return _Stream.Length; }
        }

        public long offset
        {
            get { return _Stream.Position; }
        }

        private void DebugInfo(string s)
        {
            if (DebugMode)
            {
                Debug.WriteLine(string.Format("{0:x8}", _Stream.Position) + ": " + s);
            }
        }

        public byte[] GetBytes(int length)
        {
            return _Reader.ReadBytes(length);
        }

        public float getFloat32()
        {
            return _Reader.ReadSingle();
        }

        public double getFloat64()
        {
            return _Reader.ReadDouble();
        }

        public short getInt16()
        {
            return _Reader.ReadInt16();
        }

        public int getInt32()
        {
            return _Reader.ReadInt32();
        }

        public string getString()
        {
            var len = getInt32();
            return getString(len);
        }

        public string getString(int len)
        {
            return new string(_Reader.ReadChars(len));
        }

        public ushort getUint16()
        {
            return _Reader.ReadUInt16();
        }

        public uint getUInt32()
        {
            return _Reader.ReadUInt32();
        }

        public byte getUint8()
        {
            return _Reader.ReadByte();
        }

        public int GetVarints()
        {
            byte num;
            var num2 = 0;
            var num3 = 0;
            do
            {
                num = _Reader.ReadByte();
                num2 |= (num & 0x7f) << num3;
                num3 += 7;
            } while ((num & 0x80) != 0);
            return num2;
        }

        public void seek(long offset)
        {
            if (offset > 0L)
            {
                _Stream.Seek(offset, SeekOrigin.Begin);
            }
            else
            {
                _Stream.Seek(offset, SeekOrigin.End);
            }
            DebugInfo("Seek");
        }

        public void Write(byte[] buffer)
        {
            DebugInfo("Write byte[" + buffer.Length + "]");
            _Writer.Write(buffer, 0, buffer.Length);
        }

        public void Write(byte n)
        {
            DebugInfo("Write byte[" + n + "]");
            _Writer.Write(n);
        }

        public void Write(double n)
        {
            DebugInfo("Write double[" + n + "]");
            _Writer.Write(n);
        }

        public void Write(short n)
        {
            DebugInfo("Write short[" + n + "]");
            _Writer.Write(n);
        }

        public void Write(int n)
        {
            DebugInfo("Write int[" + n + "]");
            _Writer.Write(n);
        }

        public void Write(float n)
        {
            DebugInfo("Write float[" + n + "]");
            _Writer.Write(n);
        }

        public void Write(ushort n)
        {
            DebugInfo("Write ushort[" + n + "]");
            _Writer.Write(n);
        }

        public void Write(uint n)
        {
            DebugInfo("Write uint[" + n + "]");
            _Writer.Write(n);
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            DebugInfo(string.Concat("Write byte[", buffer.Length, ", ", offset, ", ", count, "]"));
            _Writer.Write(buffer, offset, count);
        }

        public void WriteString(string s)
        {
            DebugInfo("Write string[" + s + "]");
            if (s == null)
            {
                _Writer.Write(0);
            }
            else
            {
                var bytes = Encoding.UTF8.GetBytes(s);
                _Writer.Write(bytes.Length);
                _Writer.Write(bytes, 0, bytes.Length);
            }
        }

        public void WriteStringWithoutLength(string s)
        {
            DebugInfo("Write 'string[" + s + "]");
            if (!string.IsNullOrEmpty(s))
            {
                _Writer.Write(Encoding.UTF8.GetBytes(s));
            }
        }

        public void WriteVarints(uint n)
        {
            DebugInfo("Write Varints[" + n + "]");
            do
            {
                var num = n & 0x7f;
                n = n >> 7;
                if (n > 0)
                {
                    num |= 0x80;
                }
                _Writer.Write((byte) num);
            } while (n > 0);
        }
    }
}