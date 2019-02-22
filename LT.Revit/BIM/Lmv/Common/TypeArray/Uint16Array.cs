namespace BIM.Lmv.Common.TypeArray
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    internal class Uint16Array
    {
        private const int ITEM_SIZE = 2;

        public Uint16Array(int size)
        {
            this.length = size;
            this.buffer = new byte[size * 2];
        }

        public Uint16Array(byte[] buffer)
        {
            this.length = buffer.Length / 2;
            this.buffer = buffer;
        }

        public void set(ushort[] array, int offset = 0)
        {
            for (int i = 0; i < this.length; i++)
            {
                if ((offset + i) >= array.Length)
                {
                    break;
                }
                this[i] = array[offset + i];
            }
        }

        public void set(Uint16Array array, int offset = 0)
        {
            for (int i = 0; i < this.length; i++)
            {
                if ((offset + i) >= array.length)
                {
                    break;
                }
                this[i] = array[offset + i];
            }
        }

        public ushort[] array
        {
            get
            {
                ushort[] numArray = new ushort[this.length];
                for (int i = 0; i < this.length; i++)
                {
                    numArray[i] = this[i];
                }
                return numArray;
            }
        }

        public byte[] buffer { get; private set; }

        public ushort this[int index]
        {
            get{ return BitConverter.ToUInt16(this.buffer, index*2); }
            set{Buffer.BlockCopy(BitConverter.GetBytes(value), 0, this.buffer, index * 2, 2);}
        }

        public int length { get; private set; }
    }
}

