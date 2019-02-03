using System;

namespace BIM.Lmv.Revit.Config
{
    [Serializable]
    internal class AppCloudConfig
    {
        public AppCloudConfig()
        {
            UserName = string.Empty;
            UserPassword = string.Empty;
            UserRemember = true;
            IncludeTexture = true;
            IncludeProperty = true;
            ShareMode = 1;
            ShareExpireDays = 0;
            WXToken = string.Empty;
        }

        public bool IncludeProperty { get; set; }

        public bool IncludeTexture { get; set; }

        public int ShareExpireDays { get; set; }

        public int ShareMode { get; set; }

        public string UserName { get; set; }

        public string UserPassword { get; set; }

        public bool UserRemember { get; set; }

        public string WXToken { get; set; }

        public AppCloudConfig Clone()
        {
            return new AppCloudConfig
            {
                UserName = UserName,
                UserPassword = UserPassword,
                UserRemember = UserRemember,
                IncludeTexture = IncludeTexture,
                IncludeProperty = IncludeProperty,
                ShareMode = ShareMode,
                ShareExpireDays = ShareExpireDays,
                WXToken = WXToken
            };
        }
    }
}