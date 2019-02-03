using System;
using BIM.Lmv.Common.Pack;

namespace BIM.Lmv.Content.Geometry
{
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
            var stream = pfw.stream;
            stream.Write((byte) 0);
            for (var i = 0; i < 6; i++)
            {
                stream.Write(0f);
            }
            stream.Write(primCount);
            pfw.WriteString(packFile);
            pfw.WriteU32V(entityIndex);
            stream.Write(-1);
        }
    }
}