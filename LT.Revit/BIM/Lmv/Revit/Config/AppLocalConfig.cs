using System;

namespace BIM.Lmv.Revit.Config
{
    [Serializable]
    internal class AppLocalConfig
    {
        public AppLocalConfig()
        {
            IncludeTexture = true;
            IncludeProperty = true;
            LastTargetPath = string.Empty;
        }

        public bool IncludeProperty { get; set; }

        public bool IncludeTexture { get; set; }

        public string LastTargetPath { get; set; }

        public AppLocalConfig Clone()
        {
            return new AppLocalConfig
            {
                IncludeTexture = IncludeTexture,
                IncludeProperty = IncludeProperty,
                LastTargetPath = LastTargetPath
            };
        }
    }
}