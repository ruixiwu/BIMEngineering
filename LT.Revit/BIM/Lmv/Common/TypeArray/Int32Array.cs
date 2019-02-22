namespace BIM.Lmv.Common.TypeArray
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    internal class Int32Array
    {
        private const int ITEM_SIZE = 4;

        public Int32Array(int size)
        {
            this.length = size;
            this.buffer = new byte[size * 4];
        }

        public Int32Array(byte[] buffer)
        {
            this.length = buffer.Length / 4;
            this.buffer = buffer;
        }

        public void set(int[] array, int offset = 0)
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

        public void set(Int32Array array, int offset = 0)
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

        public int[] array
        {
            get
            {
                int[] numArray = new int[this.length];
                for (int i = 0; i < this.length; i++)
                {
                    numArray[i] = this[i];
                }
                return numArray;
            }
        }

        public byte[] buffer { get; private set; }

        public int this[int index]
        {
            get { return BitConverter.ToInt32(buffer, index * 4); }
            set { Buffer.BlockCopy(BitConverter.GetBytes(value), 0, buffer, index * 4, 4); }
        }

        public int length { get; }
    }
}

