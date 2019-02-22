namespace BIM.Lmv.Content.Geometry.Types
{
    using System;

    public class Vector4F
    {
        public float w;
        public float x;
        public float y;
        public float z;

        public Vector4F(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public Vector4F Clone() => 
            new Vector4F(this.x, this.y, this.z, this.w);
    }
}

