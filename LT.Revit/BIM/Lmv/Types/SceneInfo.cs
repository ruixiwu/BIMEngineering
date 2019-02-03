using BIM.Lmv.Content.Geometry.Types;

namespace BIM.Lmv.Types
{
    public class SceneInfo
    {
        public double AngleToTrueNorth;
        public double Latitude;
        public double Longitude;
        public int VertexLimit;

        public SceneInfo()
        {
            WorldUp = new Vector3F(0f, 0f, 1f);
            WorldFront = new Vector3F(0f, -1f, 0f);
            WorldNorth = new Vector3F(0f, 1f, 0f);
            DistanceUnit = "foot";
            Longitude = 116.38765;
            Latitude = 39.90657;
            AngleToTrueNorth = 0.0;
            VertexLimit = 0xffff;
        }

        public string DistanceUnit { get; set; }

        public Vector3F WorldFront { get; set; }

        public Vector3F WorldNorth { get; set; }

        public Vector3F WorldUp { get; set; }
    }
}