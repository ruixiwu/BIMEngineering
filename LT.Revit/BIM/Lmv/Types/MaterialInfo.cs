using System.Collections.Generic;
using System.Drawing;
using BIM.Lmv.Content.Geometry.Types;

namespace BIM.Lmv.Types
{
    public class MaterialInfo
    {
        private static readonly Vector3D DefaultAmbient = ToVector3F(ColorTranslator.FromHtml("#030303"));
        private static readonly Vector3D DefaultColor = ToVector3F(ColorTranslator.FromHtml("#777777"));
        private static readonly Vector3D DefaultSpecular = ToVector3F(ColorTranslator.FromHtml("#333333"));

        public MaterialInfo()
        {
            Ambient = null;
            Color = null;
            Specular = null;
            Emissive = null;
            Shininess = 30.0;
            Transparent = 0.0;
            Reflectivity = 0.0;
            BumpIsNormal = false;
            BumpAmount = 0.0;
            IsMetal = false;
            BackfaceCulling = false;
        }

        public Vector3D Ambient { get; set; }

        public bool BackfaceCulling { get; set; }

        public double BumpAmount { get; set; }

        public bool BumpIsNormal { get; set; }

        public Vector3D Color { get; set; }

        public Vector3D Emissive { get; set; }

        public bool IsMetal { get; set; }

        public double Reflectivity { get; set; }

        public double Shininess { get; set; }

        public Vector3D Specular { get; set; }

        public List<TextureInfo> Textures { get; set; }

        public double Transparent { get; set; }

        public static Vector3D ToVector3F(Color c)
        {
            return new Vector3D(c.R/255.0, c.G/255.0, c.B/255.0);
        }
    }
}