namespace BIM.Lmv.Revit.Helpers
{
    using System;
    using System.Runtime.CompilerServices;

    internal class MaterialItem
    {
        public MaterialItem()
        {
        }

        public MaterialItem(string sName, string sTechdes, string sAmbient, string sEmission, string sShininess, string sSpecular, string sDiffuse, string sTexture1)
        {
            this.Name = sName;
            this.Techdes = sTechdes;
            this.Ambient = sAmbient;
            this.Emission = sEmission;
            this.Shininess = sShininess;
            this.Specular = sSpecular;
            this.Diffuse = sDiffuse;
            this.Texture1 = sTexture1;
        }

        public string Ambient { get; set; }

        public string Diffuse { get; set; }

        public string Emission { get; set; }

        public string Name { get; set; }

        public string ObjectId { get; set; }

        public string Shininess { get; set; }

        public string Specular { get; set; }

        public string Techdes { get; set; }

        public string Texture1 { get; set; }
    }
}

