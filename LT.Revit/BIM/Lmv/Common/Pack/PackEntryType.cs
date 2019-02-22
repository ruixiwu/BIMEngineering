namespace BIM.Lmv.Common.Pack
{
    using System;
    using System.Runtime.CompilerServices;

    internal class PackEntryType
    {
        public PackEntryType(int index, string entryClass, string entryType, int version)
        {
            this.index = index;
            this.entryClass = entryClass;
            this.entryType = entryType;
            this.version = version;
        }

        public static PackEntryType Read(PackFileStreamReader reader, int index)
        {
            string entryClass = reader.readString();
            string entryType = reader.readString();
            return new PackEntryType(index, entryClass, entryType, reader.readU32V());
        }

        public bool Write(PackFileStreamWriter writer)
        {
            writer.WriteString(this.entryClass);
            writer.WriteString(this.entryType);
            writer.WriteU32V((uint) this.version);
            return true;
        }

        public string entryClass { get; private set; }

        public string entryType { get; private set; }

        public int index { get; set; }

        public int version { get; private set; }
    }
}

