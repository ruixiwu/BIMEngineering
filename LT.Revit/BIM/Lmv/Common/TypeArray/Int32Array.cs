using System;

namespace BIM.Lmv.Common.TypeArray
{
    internal class Int32Array
    {
        private const int ITEM_SIZE = 4;

        public Int32Array(int size)
        {
            length = size;
            buffer = new byte[size*4];
        }

        public Int32Array(byte[] buffer)
        {
            length = buffer.Length/4;
            this.buffer = buffer;
        }

        public int[] array
        {
            get
            {
                var numArray = new int[length];
                for (var i = 0; i < length; i++)
                {
                    numArray[i] = this[i];
                }
                return numArray;
            }
        }

        public byte[] buffer { get; }

        public int this[int index]
        {
            get { return BitConverter.ToInt32(buffer, index*4); }
            set { Buffer.BlockCopy(BitConverter.GetBytes(value), 0, buffer, index*4, 4); }
        }

        public int length { get; }

        public void set(int[] array, int offset = 0)
        {
            for (var i = 0; i < length; i++)
            {
                if (offset + i >= array.Length)
                {
                    break;
                }
                this[i] = array[offset + i];
            }
        }

        public void set(Int32Array array, int offset = 0)
        {
            for (var i = 0; i < length; i++)
            {
                if (offset + i >= array.length)
                {
                    break;
                }
                this[i] = array[offset + i];
            }
        }
    }
}