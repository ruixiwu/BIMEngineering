using System;

namespace BIM.Lmv.Common.TypeArray
{
    internal class Uint16Array
    {
        private const int ITEM_SIZE = 2;

        public Uint16Array(int size)
        {
            length = size;
            buffer = new byte[size*2];
        }

        public Uint16Array(byte[] buffer)
        {
            length = buffer.Length/2;
            this.buffer = buffer;
        }

        public ushort[] array
        {
            get
            {
                var numArray = new ushort[length];
                for (var i = 0; i < length; i++)
                {
                    numArray[i] = this[i];
                }
                return numArray;
            }
        }

        public byte[] buffer { get; }

        public ushort this[int index]
        {
            get { return BitConverter.ToUInt16(buffer, index*2); }
            set { Buffer.BlockCopy(BitConverter.GetBytes(value), 0, buffer, index*2, 2); }
        }

        public int length { get; }

        public void set(ushort[] array, int offset = 0)
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

        public void set(Uint16Array array, int offset = 0)
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