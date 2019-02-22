namespace BIM.Lmv.Revit.Config
{
    using System;
    using System.Runtime.CompilerServices;

    [Serializable]
    internal class AppCloudConfig
    {
        public AppCloudConfig()
        {
            this.UserName = string.Empty;
            this.UserPassword = string.Empty;
            this.UserRemember = true;
            this.IncludeTexture = true;
            this.IncludeProperty = true;
            this.ShareMode = 1;
            this.ShareExpireDays = 0;
            this.WXToken = string.Empty;
        }

        public AppCloudConfig Clone() => 
            new AppCloudConfig { 
                UserName = this.UserName,
                UserPassword = this.UserPassword,
                UserRemember = this.UserRemember,
                IncludeTexture = this.IncludeTexture,
                IncludeProperty = this.IncludeProperty,
                ShareMode = this.ShareMode,
                ShareExpireDays = this.ShareExpireDays,
                WXToken = this.WXToken
            };

        public bool IncludeProperty { get; set; }

        public bool IncludeTexture { get; set; }

        public int ShareExpireDays { get; set; }

        public int ShareMode { get; set; }

        public string UserName { get; set; }

        public string UserPassword { get; set; }

        public bool UserRemember { get; set; }

        public string WXToken { get; set; }
    }
}

