using BIM.Lmv.Common.Pack;
using BIM.Lmv.Content.Geometry.Types;

namespace BIM.Lmv.Content.Other
{
    internal class EntryCamera : PackEntryBase
    {
        public float aspect;
        public float fov;
        public byte isPerspective;
        public float orthoScale;
        public Vector3F position;
        public Vector3F target;
        public Vector3F up;

        public override void Read(PackFileStreamWriter pfr, PackEntryType tse)
        {
            var stream = pfr.stream;
            isPerspective = stream.getUint8();
            position = pfr.ReadVector3F();
            target = pfr.ReadVector3F();
            up = pfr.ReadVector3F();
            aspect = stream.getFloat32();
            fov = (float) (stream.getFloat32()*57.295779513082323);
            orthoScale = stream.getFloat32();
        }

        public override void Write(PackFileStreamWriter pfw, PackEntryType tse)
        {
            var stream = pfw.stream;
            stream.Write(isPerspective);
            pfw.WriteVector3F(position);
            pfw.WriteVector3F(target);
            pfw.WriteVector3F(up);
            stream.Write(aspect);
            stream.Write((float) (fov/57.295779513082323));
            stream.Write(orthoScale);
        }
    }
}