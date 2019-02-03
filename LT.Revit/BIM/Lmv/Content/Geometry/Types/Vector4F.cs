namespace BIM.Lmv.Content.Geometry.Types
{
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

        public Vector4F Clone()
        {
            return new Vector4F(x, y, z, w);
        }
    }
}