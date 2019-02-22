namespace BIM.Lmv.Revit.Config
{
    using BIM.Lmv.Revit.Utility;
    using Newtonsoft.Json;
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Security.Cryptography;
    using System.Text;

    internal static class AppConfigManager
    {
        private const string FILE_NAME = "BIM.Lmv.Revit.cfg";
        private const string IV = "349kj*&(";
        private const string KEY = "fukgdgf#";

        public static AppConfig Load()
        {
            AppConfig config2;
            try
            {
                DESCryptoServiceProvider provider = new DESCryptoServiceProvider {
                    Key = Encoding.ASCII.GetBytes("fukgdgf#"),
                    IV = Encoding.ASCII.GetBytes("349kj*&(")
                };
                using (FileStream stream = new FileStream(AppHelper.GetPath("BIM.Lmv.Revit.cfg"), FileMode.Open, FileAccess.Read))
                {
                    using (CryptoStream stream2 = new CryptoStream(stream, provider.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        using (StreamReader reader = new StreamReader(stream2, Encoding.UTF8))
                        {
                            AppConfig config = JsonConvert.DeserializeObject<AppConfig>(reader.ReadToEnd());
                            if (config.Local == null)
                            {
                                config.Local = new AppLocalConfig();
                            }
                            if (config.Cloud == null)
                            {
                                config.Cloud = new AppCloudConfig();
                            }
                            if (config.License == null)
                            {
                                config.License = new LicenseConfig();
                            }
                            config2 = config;
                        }
                    }
                }
            }
            catch (Exception)
            {
                config2 = new AppConfig();
            }
            return config2;
        }

        public static bool Save(this AppConfig config)
        {
            bool flag;
            try
            {
                DESCryptoServiceProvider provider = new DESCryptoServiceProvider {
                    Key = Encoding.ASCII.GetBytes("fukgdgf#"),
                    IV = Encoding.ASCII.GetBytes("349kj*&(")
                };
                using (FileStream stream = new FileStream(AppHelper.GetPath("BIM.Lmv.Revit.cfg"), FileMode.Create, FileAccess.Write))
                {
                    using (CryptoStream stream2 = new CryptoStream(stream, provider.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        using (StreamWriter writer = new StreamWriter(stream2, Encoding.UTF8))
                        {
                            string str2 = JsonConvert.SerializeObject(config);
                            writer.Write(str2);
                            writer.Flush();
                        }
                    }
                    flag = true;
                }
            }
            catch (Exception)
            {
                flag = false;
            }
            return flag;
        }
    }
}

