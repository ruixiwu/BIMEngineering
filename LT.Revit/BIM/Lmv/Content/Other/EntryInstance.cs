namespace BIM.Lmv.Content.Other
{
    using BIM.Lmv.Common.Pack;
    using BIM.Lmv.Content.Geometry.Types;
    using System;

    internal class EntryInstance : PackEntryBase
    {
        public uint definition;
        public Transform transform;

        public override void Read(PackFileStreamWriter pfw, PackEntryType tse)
        {
            pfw.stream.getUint8();
            this.definition = pfw.stream.getUInt32();
            this.transform = Transform.Read(pfw);
        }

        public override void Write(PackFileStreamWriter pfw, PackEntryType tse)
        {
            pfw.WriteU8(1);
            pfw.stream.Write(this.definition);
            Transform.Write(pfw, this.transform);
        }
    }
}

