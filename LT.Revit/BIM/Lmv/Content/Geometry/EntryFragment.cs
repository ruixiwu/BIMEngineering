namespace BIM.Lmv.Content.Geometry
{
    using BIM.Lmv.Common.Pack;
    using BIM.Lmv.Content.Geometry.Types;
    using System;

    internal class EntryFragment : PackEntryBase
    {
        public readonly float[] boxes = new float[6];
        public uint dbId;
        public uint materialId;
        public uint metadataId;
        public Transform transform;

        public override void Read(PackFileStreamWriter pfr, PackEntryType tse)
        {
            PackFileStream stream = pfr.stream;
            pfr.readU8();
            this.materialId = (uint) pfr.readU32V();
            this.metadataId = (uint) pfr.readU32V();
            this.transform = Transform.Read(pfr);
            for (int i = 0; i < this.boxes.Length; i++)
            {
                this.boxes[i] = stream.getFloat32();
            }
            this.dbId = (uint) pfr.readU32V();
        }

        public override void Write(PackFileStreamWriter pfw, PackEntryType tse)
        {
            PackFileStream stream = pfw.stream;
            pfw.WriteU8(1);
            pfw.WriteU32V(this.materialId);
            pfw.WriteU32V(this.metadataId);
            Transform.Write(pfw, this.transform);
            for (int i = 0; i < this.boxes.Length; i++)
            {
                stream.Write(this.boxes[i]);
            }
            pfw.WriteU32V(this.dbId);
        }
    }
}

