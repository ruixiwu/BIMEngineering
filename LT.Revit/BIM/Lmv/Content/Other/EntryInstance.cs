using BIM.Lmv.Common.Pack;
using BIM.Lmv.Content.Geometry.Types;

namespace BIM.Lmv.Content.Other
{
    internal class EntryInstance : PackEntryBase
    {
        public uint definition;
        public Transform transform;

        public override void Read(PackFileStreamWriter pfw, PackEntryType tse)
        {
            pfw.stream.getUint8();
            definition = pfw.stream.getUInt32();
            transform = Transform.Read(pfw);
        }

        public override void Write(PackFileStreamWriter pfw, PackEntryType tse)
        {
            pfw.WriteU8(1);
            pfw.stream.Write(definition);
            Transform.Write(pfw, transform);
        }
    }
}