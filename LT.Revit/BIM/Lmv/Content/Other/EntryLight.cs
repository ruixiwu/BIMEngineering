using BIM.Lmv.Common.Pack;
using BIM.Lmv.Content.Geometry.Types;

namespace BIM.Lmv.Content.Other
{
    internal class EntryLight : PackEntryBase
    {
        public float b;
        public Vector3F dir;
        public float g;
        public float intensity;
        public Vector3F position;
        public float r;
        public float size;
        public float spotAngle;
        public byte type;

        public override void Read(PackFileStreamWriter pfr, PackEntryType tse)
        {
            var stream = pfr.stream;
            position = pfr.ReadVector3F();
            dir = pfr.ReadVector3F();
            r = stream.getFloat32();
            g = stream.getFloat32();
            b = stream.getFloat32();
            intensity = stream.getFloat32();
            spotAngle = stream.getFloat32();
            size = stream.getFloat32();
            type = stream.getUint8();
        }

        public override void Write(PackFileStreamWriter pfw, PackEntryType tse)
        {
            var stream = pfw.stream;
            pfw.WriteVector3F(position);
            pfw.WriteVector3F(dir);
            stream.Write(r);
            stream.Write(g);
            stream.Write(b);
            stream.Write(intensity);
            stream.Write(spotAngle);
            stream.Write(size);
            stream.Write(type);
        }
    }
}