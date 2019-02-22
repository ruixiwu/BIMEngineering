namespace BIM.Lmv.Revit.Config
{
    using System;
    using System.Runtime.CompilerServices;

    [Serializable]
    internal class AppConfig
    {
        public AppConfig()
        {
            this.Cloud = new AppCloudConfig();
            this.Local = new AppLocalConfig();
            this.License = new LicenseConfig();
        }

        public AppConfig Clone() => 
            new AppConfig { 
                Cloud = (this.Cloud == null) ? new AppCloudConfig() : this.Cloud.Clone(),
                Local = (this.Local == null) ? new AppLocalConfig() : this.Local.Clone(),
                License = (this.License == null) ? new LicenseConfig() : this.License.Clone()
            };

        public AppCloudConfig Cloud { get; set; }

        public LicenseConfig License { get; set; }

        public AppLocalConfig Local { get; set; }
    }
}

