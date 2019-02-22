namespace BIM.Lmv.Content.Other
{
    using BIM.Lmv.Common.Pack;
    using BIM.Lmv.Content.Geometry.Types;
    using System;

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
            PackFileStream stream = pfr.stream;
            this.isPerspective = stream.getUint8();
            this.position = pfr.ReadVector3F();
            this.target = pfr.ReadVector3F();
            this.up = pfr.ReadVector3F();
            this.aspect = stream.getFloat32();
            this.fov = (float) (stream.getFloat32() * 57.295779513082323);
            this.orthoScale = stream.getFloat32();
        }

        public override void Write(PackFileStreamWriter pfw, PackEntryType tse)
        {
            PackFileStream stream = pfw.stream;
            stream.Write(this.isPerspective);
            pfw.WriteVector3F(this.position);
            pfw.WriteVector3F(this.target);
            pfw.WriteVector3F(this.up);
            stream.Write(this.aspect);
            stream.Write((float) (((double) this.fov) / 57.295779513082323));
            stream.Write(this.orthoScale);
        }
    }
}

