namespace BIM.Lmv.Revit.Config
{
    using System;
    using System.Runtime.CompilerServices;

    [Serializable]
    internal class LicenseConfig
    {
        public LicenseConfig()
        {
            this.LicenseMode = "Trial";
            this.LicenseServer = "";
        }

        public LicenseConfig Clone() => 
            new LicenseConfig { 
                LicenseMode = this.LicenseMode,
                LicenseServer = this.LicenseServer
            };

        public string LicenseMode { get; set; }

        public string LicenseServer { get; set; }
    }
}

