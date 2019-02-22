namespace BIM.Lmv.Common.TypeArray
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    internal class UInt8Array
    {
        private const int ITEM_SIZE = 1;

        public UInt8Array(int size)
        {
            this.length = size;
            this.buffer = new byte[size];
        }

        public UInt8Array(byte[] buffer)
        {
            this.length = buffer.Length / 1;
            this.buffer = buffer;
        }

        public void set(byte[] array, int offset = 0)
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

        public void set(UInt8Array array, int offset = 0)
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

        public byte[] array =>
            this.buffer;

        public byte[] buffer { get; private set; }

        public byte this[int index]
        {
            get { return this.buffer[index]; }
            set { this.buffer[index] = value; }
        }

        public int length { get; private set; }
    }
}

