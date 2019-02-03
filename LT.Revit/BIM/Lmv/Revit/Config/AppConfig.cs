using System;

namespace BIM.Lmv.Revit.Config
{
    [Serializable]
    internal class AppConfig
    {
        public AppConfig()
        {
            Cloud = new AppCloudConfig();
            Local = new AppLocalConfig();
            License = new LicenseConfig();
        }

        public AppCloudConfig Cloud { get; set; }

        public LicenseConfig License { get; set; }

        public AppLocalConfig Local { get; set; }

        public AppConfig Clone()
        {
            return new AppConfig
            {
                Cloud = Cloud == null ? new AppCloudConfig() : Cloud.Clone(),
                Local = Local == null ? new AppLocalConfig() : Local.Clone(),
                License = License == null ? new LicenseConfig() : License.Clone()
            };
        }
    }
}