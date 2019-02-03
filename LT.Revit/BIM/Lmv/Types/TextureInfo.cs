using System;

namespace BIM.Lmv.Types
{
    public class TextureInfo
    {
        public TextureInfo()
        {
            TextureType = TextureType.Diffuse;
            Invert = false;
            URepeat = true;
            VRepeat = true;
            UScale = 1.0;
            VScale = 1.0;
            UOffset = 0.0;
            VOffset = 0.0;
            WAngle = 0.0;
            IsShared = false;
        }

        public bool Invert { get; set; }

        public bool IsShared { get; set; }

        public string TextureFilePath { get; set; }

        public TextureType TextureType { get; set; }

        public double UOffset { get; set; }

        public bool URepeat { get; set; }

        public double UScale { get; set; }

        public double VOffset { get; set; }

        public bool VRepeat { get; set; }

        public double VScale { get; set; }

        public double WAngle { get; set; }

        public string GetTextureTypeText()
        {
            switch (TextureType)
            {
                case TextureType.Diffuse:
                    return "generic_diffuse";

                case TextureType.Bump:
                    return "generic_bump";

                case TextureType.Specular:
                    return "generic_specular";

                case TextureType.Alpha:
                    return "generic_alpha";
            }
            throw new ArgumentOutOfRangeException();
        }
    }
}