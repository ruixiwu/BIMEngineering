namespace BIM.Lmv.Common.Pack
{
    using System;

    internal abstract class PackEntryBase
    {
        protected PackEntryBase()
        {
        }

        public abstract void Read(PackFileStreamWriter pfw, PackEntryType tse);
        public abstract void Write(PackFileStreamWriter pfw, PackEntryType tse);
    }
}

