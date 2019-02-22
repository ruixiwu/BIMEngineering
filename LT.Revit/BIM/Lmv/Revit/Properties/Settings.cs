namespace BIM.Lmv.Revit.Properties
{
    using System;
    using System.CodeDom.Compiler;
    using System.Configuration;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    [GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "12.0.0.0"), CompilerGenerated]
    internal sealed class Settings : ApplicationSettingsBase
    {
        private static Settings defaultInstance = ((Settings) SettingsBase.Synchronized(new Settings()));

        [DefaultSettingValue("http://v.yunzu360.com/"), ApplicationScopedSetting, DebuggerNonUserCode]
        public string ApiEndpoint =>
            ((string) this["ApiEndpoint"]);

        public static Settings Default =>
            defaultInstance;

        [DebuggerNonUserCode, DefaultSettingValue("http://www.yunzu360.cn/"), ApplicationScopedSetting]
        public string WXEndpoint =>
            ((string) this["WXEndpoint"]);
    }
}

