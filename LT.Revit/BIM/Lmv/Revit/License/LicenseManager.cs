namespace BIM.Lmv.Revit.License
{
    using BIM.Lmv.Revit.Config;
    using BIM.Lmv.Revit.License.Standard;
    using License;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    internal static class LicenseManager
    {
        private static bool _IsTrial;
        private static readonly LicensePub _LicenseStandard = new LicensePub();
        private const uint FEATURE_ID = 0x21;
        public const string MODE_STANDARD = "Standard";
        public const string MODE_TRIAL = "Trial";

        public static void Check(out bool isValid, out string status)
        {
            if (_IsTrial)
            {
                CheckTrialLicense(out isValid, out status);
            }
            else
            {
                CheckStandardLicense(out isValid, out status);
            }
        }

        private static void CheckStandardLicense(out bool isValid, out string status)
        {
            try
            {
                if (_LicenseStandard.IsLogin)
                {
                    _LicenseStandard.Logout();
                }
                string licenseServer = Config.License.LicenseServer;
                if (_LicenseStandard.LoginEx(0x21, ref licenseServer))
                {
                    isValid = true;
                    status = "授权检查成功!";
                    Config.License.LicenseServer = licenseServer;
                }
                else
                {
                    isValid = false;
                    status = _LicenseStandard.ErrorDescribe;
                }
            }
            catch (Exception exception)
            {
                isValid = false;
                status = exception.ToString();
            }
        }

        public static void CheckTrialLicense(out bool isValid, out string status)
        {
            try
            {
                List<string> values = new List<string>();
                bool flag = false;
                if (Status.Hardware_Lock_Enabled && (Status.License_HardwareID != Status.HardwareID))
                {
                    isValid = false;
                    status = "授权无效!";
                    return;
                }
                if (Status.Expiration_Date_Lock_Enable)
                {
                    if (DateTime.Today > Status.Expiration_Date)
                    {
                        isValid = false;
                        status = $"有效期至 {Status.Expiration_Date:yyyy-MM-dd}";
                        return;
                    }
                    values.Add($"有效期至 {Status.Expiration_Date:yyyy-MM-dd}");
                    flag = true;
                }
                if (!Status.Evaluation_Lock_Enabled)
                {
                    goto Label_01B7;
                }
                switch (Status.Evaluation_Type)
                {
                    case EvaluationType.Trial_Days:
                        if (Status.Evaluation_Time_Current <= Status.Evaluation_Time)
                        {
                            break;
                        }
                        isValid = false;
                        status = $"共可用 {Status.Evaluation_Time} 天, 已用 {Status.Evaluation_Time_Current} 天, 剩余 {Status.Evaluation_Time - Status.Evaluation_Time_Current} 天";
                        return;

                    case EvaluationType.Runtime_Minutes:
                        if (Status.Evaluation_Time_Current <= Status.Evaluation_Time)
                        {
                            goto Label_0165;
                        }
                        isValid = false;
                        status = $"共可用 {Status.Evaluation_Time} 分钟, 已用 {Status.Evaluation_Time_Current} 分钟, 剩余 {Status.Evaluation_Time - Status.Evaluation_Time_Current} 分钟";
                        return;

                    default:
                        throw new NotSupportedException("Evaluation_Type: " + Status.Evaluation_Type);
                }
                values.Add($"共可用 {Status.Evaluation_Time} 天, 已用 {Status.Evaluation_Time_Current} 天, 剩余 {Status.Evaluation_Time - Status.Evaluation_Time_Current} 天");
                goto Label_01B5;
            Label_0165:
                values.Add($"共可用 {Status.Evaluation_Time} 分钟, 已用 {Status.Evaluation_Time_Current} 分钟, 剩余 {Status.Evaluation_Time - Status.Evaluation_Time_Current} 分钟");
            Label_01B5:
                flag = true;
            Label_01B7:
                if (Status.Number_Of_Uses_Lock_Enable)
                {
                    if (Status.Number_Of_Uses_Current > Status.Number_Of_Uses)
                    {
                        isValid = false;
                        status = $"共可用 {Status.Number_Of_Uses} 次, 已用 {Status.Number_Of_Uses_Current} 次, 剩余 {Status.Number_Of_Uses - Status.Number_Of_Uses_Current} 次";
                        return;
                    }
                    values.Add($"共可用 {Status.Number_Of_Uses} 次, 已用 {Status.Number_Of_Uses_Current} 次, 剩余 {Status.Number_Of_Uses - Status.Number_Of_Uses_Current} 次");
                    flag = true;
                }
                if (Status.Licensed)
                {
                    isValid = true;
                    status = (values.Count == 0) ? "免费授权" : string.Join(";", values);
                }
                else if (flag)
                {
                    isValid = true;
                    status = (values.Count == 0) ? "试用授权" : string.Join(";", values);
                }
                else
                {
                    isValid = false;
                    status = "无授权信息或授权已过期!";
                }
            }
            catch (Exception exception)
            {
                isValid = false;
                status = exception.Message;
            }
        }

        public static void End()
        {
            if (!_IsTrial && _LicenseStandard.IsLogin)
            {
                _LicenseStandard.Logout();
            }
        }

        public static void Init(AppConfig config)
        {
            End();
            Config = config;
            _IsTrial = Config.License.LicenseMode == "Trial";
        }

        public static bool Start()
        {
            End();
            if (_IsTrial)
            {
                bool flag;
                string str;
                CheckTrialLicense(out flag, out str);
                IsValid = flag;
                Message = IsValid ? null : "授权无效!";
            }
            else
            {
                if (_LicenseStandard.IsLogin)
                {
                    _LicenseStandard.Logout();
                }
                string licenseServer = Config.License.LicenseServer;
                if (_LicenseStandard.LoginEx(0x21, ref licenseServer))
                {
                    IsValid = true;
                    Message = null;
                    if (Config.License.LicenseServer != licenseServer)
                    {
                        Config.License.LicenseServer = licenseServer;
                        Config.Save();
                    }
                }
                else
                {
                    IsValid = false;
                    Message = _LicenseStandard.ErrorDescribe;
                }
            }
            return IsValid;
        }

        public static AppConfig Config
        {
            [CompilerGenerated]
            get => 
                <Config>k__BackingField;
            [CompilerGenerated]
            private set
            {
                <Config>k__BackingField = value;
            }
        }

        public static bool IsValid
        {
            [CompilerGenerated]
            get => 
                <IsValid>k__BackingField;
            [CompilerGenerated]
            private set
            {
                <IsValid>k__BackingField = value;
            }
        }

        public static string Message
        {
            [CompilerGenerated]
            get => 
                <Message>k__BackingField;
            [CompilerGenerated]
            private set
            {
                <Message>k__BackingField = value;
            }
        }
    }
}

