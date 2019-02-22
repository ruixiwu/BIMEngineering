namespace BIM.Lmv.Types
{
    using BIM.Lmv.Content.Geometry.Types;
    using System;
    using System.Runtime.CompilerServices;

    public class SceneInfo
    {
        public double AngleToTrueNorth;
        public double Latitude;
        public double Longitude;
        public int VertexLimit;

        public SceneInfo()
        {
            this.WorldUp = new Vector3F(0f, 0f, 1f);
            this.WorldFront = new Vector3F(0f, -1f, 0f);
            this.WorldNorth = new Vector3F(0f, 1f, 0f);
            this.DistanceUnit = "foot";
            this.Longitude = 116.38765;
            this.Latitude = 39.90657;
            this.AngleToTrueNorth = 0.0;
            this.VertexLimit = 0xffff;
        }

        public string DistanceUnit { get; set; }

        public Vector3F WorldFront { get; set; }

        public Vector3F WorldNorth { get; set; }

        public Vector3F WorldUp { get; set; }
    }
}

