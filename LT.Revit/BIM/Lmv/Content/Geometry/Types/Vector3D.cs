namespace BIM.Lmv.Content.Geometry.Types
{
    public class Vector3D
    {
        public double x;
        public double y;
        public double z;

        public Vector3D()
        {
        }

        public Vector3D(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3D Clone()
        {
            return new Vector3D(x, y, z);
        }

        public static explicit operator Vector3D(Vector3F v)
        {
            if (v == null)
            {
                return null;
            }
            return new Vector3D(v.x, v.y, v.z);
        }

        public override string ToString()
        {
            return string.Concat("Viector3D(", x, ",", y, ",", z, ")");
        }
    }
}