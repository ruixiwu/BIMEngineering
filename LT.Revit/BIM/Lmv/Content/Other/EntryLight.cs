namespace BIM.Lmv.Content.Other
{
    using BIM.Lmv.Common.Pack;
    using BIM.Lmv.Content.Geometry.Types;
    using System;

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
            PackFileStream stream = pfr.stream;
            this.position = pfr.ReadVector3F();
            this.dir = pfr.ReadVector3F();
            this.r = stream.getFloat32();
            this.g = stream.getFloat32();
            this.b = stream.getFloat32();
            this.intensity = stream.getFloat32();
            this.spotAngle = stream.getFloat32();
            this.size = stream.getFloat32();
            this.type = stream.getUint8();
        }

        public override void Write(PackFileStreamWriter pfw, PackEntryType tse)
        {
            PackFileStream stream = pfw.stream;
            pfw.WriteVector3F(this.position);
            pfw.WriteVector3F(this.dir);
            stream.Write(this.r);
            stream.Write(this.g);
            stream.Write(this.b);
            stream.Write(this.intensity);
            stream.Write(this.spotAngle);
            stream.Write(this.size);
            stream.Write(this.type);
        }
    }
}

