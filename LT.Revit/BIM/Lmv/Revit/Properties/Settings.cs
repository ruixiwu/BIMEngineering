using System.CodeDom.Compiler;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace BIM.Lmv.Revit.Properties
{
    [GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "12.0.0.0"),
     CompilerGenerated]
    internal sealed class Settings : ApplicationSettingsBase
    {
        [ApplicationScopedSetting, DebuggerNonUserCode, DefaultSettingValue("http://v.yunzu360.com/")]
        public string ApiEndpoint
        {
            get { return (string) this["ApiEndpoint"]; }
        }

        public static Settings Default { get; } = (Settings) Synchronized(new Settings());

        [DefaultSettingValue("http://www.yunzu360.cn/"), ApplicationScopedSetting, DebuggerNonUserCode]
        public string WXEndpoint
        {
            get { return (string) this["WXEndpoint"]; }
        }
    }
}