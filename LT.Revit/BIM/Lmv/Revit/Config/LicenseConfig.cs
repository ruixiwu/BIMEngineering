using System;

namespace BIM.Lmv.Revit.Config
{
    [Serializable]
    internal class LicenseConfig
    {
        public LicenseConfig()
        {
            LicenseMode = "Trial";
            LicenseServer = "";
        }

        public string LicenseMode { get; set; }

        public string LicenseServer { get; set; }

        public LicenseConfig Clone()
        {
            return new LicenseConfig
            {
                LicenseMode = LicenseMode,
                LicenseServer = LicenseServer
            };
        }
    }
}