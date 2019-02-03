namespace BIM.Lmv.Common.Pack
{
    internal class PackEntryType
    {
        public PackEntryType(int index, string entryClass, string entryType, int version)
        {
            this.index = index;
            this.entryClass = entryClass;
            this.entryType = entryType;
            this.version = version;
        }

        public string entryClass { get; }

        public string entryType { get; }

        public int index { get; set; }

        public int version { get; }

        public static PackEntryType Read(PackFileStreamReader reader, int index)
        {
            var entryClass = reader.readString();
            var entryType = reader.readString();
            return new PackEntryType(index, entryClass, entryType, reader.readU32V());
        }

        public bool Write(PackFileStreamWriter writer)
        {
            writer.WriteString(entryClass);
            writer.WriteString(entryType);
            writer.WriteU32V((uint) version);
            return true;
        }
    }
}