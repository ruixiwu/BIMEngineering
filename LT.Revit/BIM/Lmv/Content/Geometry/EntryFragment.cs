using BIM.Lmv.Common.Pack;
using BIM.Lmv.Content.Geometry.Types;

namespace BIM.Lmv.Content.Geometry
{
    internal class EntryFragment : PackEntryBase
    {
        public readonly float[] boxes = new float[6];
        public uint dbId;
        public uint materialId;
        public uint metadataId;
        public Transform transform;

        public override void Read(PackFileStreamWriter pfr, PackEntryType tse)
        {
            var stream = pfr.stream;
            pfr.readU8();
            materialId = (uint) pfr.readU32V();
            metadataId = (uint) pfr.readU32V();
            transform = Transform.Read(pfr);
            for (var i = 0; i < boxes.Length; i++)
            {
                boxes[i] = stream.getFloat32();
            }
            dbId = (uint) pfr.readU32V();
        }

        public override void Write(PackFileStreamWriter pfw, PackEntryType tse)
        {
            var stream = pfw.stream;
            pfw.WriteU8(1);
            pfw.WriteU32V(materialId);
            pfw.WriteU32V(metadataId);
            Transform.Write(pfw, transform);
            for (var i = 0; i < boxes.Length; i++)
            {
                stream.Write(boxes[i]);
            }
            pfw.WriteU32V(dbId);
        }
    }
}