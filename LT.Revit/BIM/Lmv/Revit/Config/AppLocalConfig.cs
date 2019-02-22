namespace BIM.Lmv.Revit.Config
{
    using System;
    using System.Runtime.CompilerServices;

    [Serializable]
    internal class AppLocalConfig
    {
        public AppLocalConfig()
        {
            this.IncludeTexture = true;
            this.IncludeProperty = true;
            this.LastTargetPath = string.Empty;
        }

        public AppLocalConfig Clone() => 
            new AppLocalConfig { 
                IncludeTexture = this.IncludeTexture,
                IncludeProperty = this.IncludeProperty,
                LastTargetPath = this.LastTargetPath
            };

        public bool IncludeProperty { get; set; }

        public bool IncludeTexture { get; set; }

        public string LastTargetPath { get; set; }
    }
}

