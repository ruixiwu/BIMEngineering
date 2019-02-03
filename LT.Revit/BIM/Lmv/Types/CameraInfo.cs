using BIM.Lmv.Content.Geometry.Types;

namespace BIM.Lmv.Types
{
    public class CameraInfo
    {
        public float Aspect { get; set; }

        public float Fov { get; set; }

        public bool IsPerspective { get; set; }

        public float OrthoScale { get; set; }

        public Vector3F Position { get; set; }

        public Vector3F Target { get; set; }

        public Vector3F Up { get; set; }
    }
}