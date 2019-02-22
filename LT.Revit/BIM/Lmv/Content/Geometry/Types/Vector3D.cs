namespace BIM.Lmv.Content.Geometry.Types
{
    using System;

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

        public Vector3D Clone() => 
            new Vector3D(this.x, this.y, this.z);

        public static explicit operator Vector3D(Vector3F v)
        {
            if (v == null)
            {
                return null;
            }
            return new Vector3D((double) v.x, (double) v.y, (double) v.z);
        }

        public override string ToString() => 
            string.Concat(new object[] { "Viector3D(", this.x, ",", this.y, ",", this.z, ")" });
    }
}

