namespace BIM.Lmv.Types
{
    using BIM.Lmv.Content.Geometry.Types;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Runtime.CompilerServices;

    public class MaterialInfo
    {
        private static readonly Vector3D DefaultAmbient = ToVector3F(ColorTranslator.FromHtml("#030303"));
        private static readonly Vector3D DefaultColor = ToVector3F(ColorTranslator.FromHtml("#777777"));
        private static readonly Vector3D DefaultSpecular = ToVector3F(ColorTranslator.FromHtml("#333333"));

        public MaterialInfo()
        {
            this.Ambient = null;
            this.Color = null;
            this.Specular = null;
            this.Emissive = null;
            this.Shininess = 30.0;
            this.Transparent = 0.0;
            this.Reflectivity = 0.0;
            this.BumpIsNormal = false;
            this.BumpAmount = 0.0;
            this.IsMetal = false;
            this.BackfaceCulling = false;
        }

        public static Vector3D ToVector3F(System.Drawing.Color c) => 
            new Vector3D(((double) c.R) / 255.0, ((double) c.G) / 255.0, ((double) c.B) / 255.0);

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
    }
}

