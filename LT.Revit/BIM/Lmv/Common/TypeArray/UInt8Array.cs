namespace BIM.Lmv.Common.TypeArray
{
    internal class UInt8Array
    {
        private const int ITEM_SIZE = 1;

        public UInt8Array(int size)
        {
            length = size;
            buffer = new byte[size];
        }

        public UInt8Array(byte[] buffer)
        {
            length = buffer.Length/1;
            this.buffer = buffer;
        }

        public byte[] array
        {
            get { return buffer; }
        }

        public byte[] buffer { get; }

        public byte this[int index]
        {
            get { return buffer[index]; }
            set { buffer[index] = value; }
        }

        public int length { get; }

        public void set(byte[] array, int offset = 0)
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

        public void set(UInt8Array array, int offset = 0)
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