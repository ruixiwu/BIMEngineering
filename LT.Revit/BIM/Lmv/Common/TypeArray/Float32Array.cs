namespace BIM.Lmv.Common.TypeArray
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    internal class Float32Array
    {
        private const int ITEM_SIZE = 4;

        public Float32Array(int size)
        {
            this.length = size;
            this.buffer = new byte[size * 4];
        }

        public Float32Array(byte[] buffer)
        {
            this.length = buffer.Length / 4;
            this.buffer = buffer;
        }

        public Float32Array(float[] array)
        {
            this.length = array.Length;
            this.buffer = new byte[this.length * 4];
            for (int i = 0; i < this.length; i++)
            {
                this[i] = array[i];
            }
        }

        public void set(float[] array, int offset = 0)
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

        public void set(Float32Array array, int offset = 0)
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

        public float[] array
        {
            get
            {
                float[] numArray = new float[this.length];
                for (int i = 0; i < this.length; i++)
                {
                    numArray[i] = this[i];
                }
                return numArray;
            }
        }

        public byte[] buffer { get; private set; }

        public float this[int index]
        {
            get { return BitConverter.ToSingle(this.buffer, index*4); }
            set
            {
                Buffer.BlockCopy(BitConverter.GetBytes(value), 0, this.buffer, index * 4, 4);
            }
        }

        public int length { get; private set; }
    }
}

