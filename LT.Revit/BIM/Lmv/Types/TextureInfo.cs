namespace BIM.Lmv.Types
{
    using System;
    using System.Runtime.CompilerServices;

    public class TextureInfo
    {
        public TextureInfo()
        {
            this.TextureType = BIM.Lmv.Types.TextureType.Diffuse;
            this.Invert = false;
            this.URepeat = true;
            this.VRepeat = true;
            this.UScale = 1.0;
            this.VScale = 1.0;
            this.UOffset = 0.0;
            this.VOffset = 0.0;
            this.WAngle = 0.0;
            this.IsShared = false;
        }

        public string GetTextureTypeText()
        {
            switch (this.TextureType)
            {
                case BIM.Lmv.Types.TextureType.Diffuse:
                    return "generic_diffuse";

                case BIM.Lmv.Types.TextureType.Bump:
                    return "generic_bump";

                case BIM.Lmv.Types.TextureType.Specular:
                    return "generic_specular";

                case BIM.Lmv.Types.TextureType.Alpha:
                    return "generic_alpha";
            }
            throw new ArgumentOutOfRangeException();
        }

        public bool Invert { get; set; }

        public bool IsShared { get; set; }

        public string TextureFilePath { get; set; }

        public BIM.Lmv.Types.TextureType TextureType { get; set; }

        public double UOffset { get; set; }

        public bool URepeat { get; set; }

        public double UScale { get; set; }

        public double VOffset { get; set; }

        public bool VRepeat { get; set; }

        public double VScale { get; set; }

        public double WAngle { get; set; }
    }
}

