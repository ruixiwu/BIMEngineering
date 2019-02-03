namespace BIM.Lmv.Common.Pack
{
    internal abstract class PackEntryBase
    {
        public abstract void Read(PackFileStreamWriter pfw, PackEntryType tse);
        public abstract void Write(PackFileStreamWriter pfw, PackEntryType tse);
    }
}