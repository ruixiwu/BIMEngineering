namespace BIM.Lmv.Common.Pack
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text;

    internal class PackFileStream
    {
        private Encoding _Encoding = Encoding.UTF8;
        private BinaryReader _Reader;
        private Stream _Stream;
        private BinaryWriter _Writer;
        public static bool DebugMode = false;

        public PackFileStream(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            this._Stream = stream;
            if (!stream.CanSeek)
            {
                throw new ArgumentException("Sstream can't seek!");
            }
            if (stream.CanWrite)
            {
                this._Writer = new BinaryWriter(stream, this._Encoding);
            }
            if (stream.CanRead)
            {
                this._Reader = new BinaryReader(stream, this._Encoding);
            }
        }

        private void DebugInfo(string s)
        {
            if (DebugMode)
            {
                Debug.WriteLine($"{this._Stream.Position:x8}" + ": " + s);
            }
        }

        public byte[] GetBytes(int length) => 
            this._Reader.ReadBytes(length);

        public float getFloat32() => 
            this._Reader.ReadSingle();

        public double getFloat64() => 
            this._Reader.ReadDouble();

        public short getInt16() => 
            this._Reader.ReadInt16();

        public int getInt32() => 
            this._Reader.ReadInt32();

        public string getString()
        {
            int len = this.getInt32();
            return this.getString(len);
        }

        public string getString(int len) => 
            new string(this._Reader.ReadChars(len));

        public ushort getUint16() => 
            this._Reader.ReadUInt16();

        public uint getUInt32() => 
            this._Reader.ReadUInt32();

        public byte getUint8() => 
            this._Reader.ReadByte();

        public int GetVarints()
        {
            byte num;
            int num2 = 0;
            int num3 = 0;
            do
            {
                num = this._Reader.ReadByte();
                num2 |= (num & 0x7f) << num3;
                num3 += 7;
            }
            while ((num & 0x80) != 0);
            return num2;
        }

        public void seek(long offset)
        {
            if (offset > 0L)
            {
                this._Stream.Seek(offset, SeekOrigin.Begin);
            }
            else
            {
                this._Stream.Seek(offset, SeekOrigin.End);
            }
            this.DebugInfo("Seek");
        }

        public void Write(byte[] buffer)
        {
            this.DebugInfo("Write byte[" + buffer.Length + "]");
            this._Writer.Write(buffer, 0, buffer.Length);
        }

        public void Write(byte n)
        {
            this.DebugInfo("Write byte[" + n + "]");
            this._Writer.Write(n);
        }

        public void Write(double n)
        {
            this.DebugInfo("Write double[" + n + "]");
            this._Writer.Write(n);
        }

        public void Write(short n)
        {
            this.DebugInfo("Write short[" + n + "]");
            this._Writer.Write(n);
        }

        public void Write(int n)
        {
            this.DebugInfo("Write int[" + n + "]");
            this._Writer.Write(n);
        }

        public void Write(float n)
        {
            this.DebugInfo("Write float[" + n + "]");
            this._Writer.Write(n);
        }

        public void Write(ushort n)
        {
            this.DebugInfo("Write ushort[" + n + "]");
            this._Writer.Write(n);
        }

        public void Write(uint n)
        {
            this.DebugInfo("Write uint[" + n + "]");
            this._Writer.Write(n);
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            this.DebugInfo(string.Concat(new object[] { "Write byte[", buffer.Length, ", ", offset, ", ", count, "]" }));
            this._Writer.Write(buffer, offset, count);
        }

        public void WriteString(string s)
        {
            this.DebugInfo("Write string[" + s + "]");
            if (s == null)
            {
                this._Writer.Write(0);
            }
            else
            {
                byte[] bytes = Encoding.UTF8.GetBytes(s);
                this._Writer.Write(bytes.Length);
                this._Writer.Write(bytes, 0, bytes.Length);
            }
        }

        public void WriteStringWithoutLength(string s)
        {
            this.DebugInfo("Write 'string[" + s + "]");
            if (!string.IsNullOrEmpty(s))
            {
                this._Writer.Write(Encoding.UTF8.GetBytes(s));
            }
        }

        public void WriteVarints(uint n)
        {
            this.DebugInfo("Write Varints[" + n + "]");
            do
            {
                uint num = n & 0x7f;
                n = n >> 7;
                if (n > 0)
                {
                    num |= 0x80;
                }
                this._Writer.Write((byte) num);
            }
            while (n > 0);
        }

        public long ByteLength =>
            this._Stream.Length;

        public long offset =>
            this._Stream.Position;
    }
}

