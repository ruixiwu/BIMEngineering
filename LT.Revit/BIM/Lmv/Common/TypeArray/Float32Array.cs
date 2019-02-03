using System;

namespace BIM.Lmv.Common.TypeArray
{
    internal class Float32Array
    {
        private const int ITEM_SIZE = 4;

        public Float32Array(int size)
        {
            length = size;
            buffer = new byte[size*4];
        }

        public Float32Array(byte[] buffer)
        {
            length = buffer.Length/4;
            this.buffer = buffer;
        }

        public Float32Array(float[] array)
        {
            length = array.Length;
            buffer = new byte[length*4];
            for (var i = 0; i < length; i++)
            {
                this[i] = array[i];
            }
        }

        public float[] array
        {
            get
            {
                var numArray = new float[length];
                for (var i = 0; i < length; i++)
                {
                    numArray[i] = this[i];
                }
                return numArray;
            }
        }

        public byte[] buffer { get; }

        public float this[int index]
        {
            get { return BitConverter.ToSingle(buffer, index*4); }
            set { Buffer.BlockCopy(BitConverter.GetBytes(value), 0, buffer, index*4, 4); }
        }

        public int length { get; }

        public void set(float[] array, int offset = 0)
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

        public void set(Float32Array array, int offset = 0)
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