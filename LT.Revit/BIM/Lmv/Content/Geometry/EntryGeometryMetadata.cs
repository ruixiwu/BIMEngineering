namespace BIM.Lmv.Content.Geometry
{
    using BIM.Lmv.Common.Pack;
    using System;

    internal class EntryGeometryMetadata : PackEntryBase
    {
        public uint entityIndex;
        public string packFile;
        public ushort primCount;

        public override void Read(PackFileStreamWriter pfw, PackEntryType tse)
        {
            throw new NotSupportedException();
        }

        public override void Write(PackFileStreamWriter pfw, PackEntryType tse)
        {
            PackFileStream stream = pfw.stream;
            stream.Write((byte) 0);
            for (int i = 0; i < 6; i++)
            {
                stream.Write((float) 0f);
            }
            stream.Write(this.primCount);
            pfw.WriteString(this.packFile);
            pfw.WriteU32V(this.entityIndex);
            stream.Write(-1);
        }
    }
}

