namespace BIM.Lmv.Revit.License
{
    using BIM.Lmv.Revit.Config;
    using BIM.Lmv.Revit.UI;
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows.Forms;

    internal class LicenseScope : IDisposable
    {
        private readonly AppConfig _AppConfig;

        public LicenseScope(AppConfig appConfig)
        {
            this._AppConfig = appConfig;
            LicenseManager.Init(this._AppConfig);
            if (this.Start())
            {
                this.IsValid = true;
            }
            else if (this.ShowForm() && this.Start())
            {
                this.IsValid = true;
            }
            else
            {
                this.IsValid = false;
            }
        }

        private void End()
        {
            LicenseManager.End();
        }

        public bool ShowForm()
        {
            FormLicenseMode mode;
            AppConfig config = this._AppConfig.Clone();
            bool flag = false;
        Label_000E:
            mode = new FormLicenseMode(config.License);
            if (mode.ShowDialog() != DialogResult.Cancel)
            {
                LicenseManager.Init(config);
                if (config.License.LicenseMode == "Trial")
                {
                    FormLicenseTrial trial = new FormLicenseTrial();
                    if (trial.ShowDialog() != DialogResult.OK)
                    {
                        goto Label_000E;
                    }
                    flag = true;
                }
                else
                {
                    FormLicense license = new FormLicense(config.License);
                    if (license.ShowDialog() != DialogResult.OK)
                    {
                        goto Label_000E;
                    }
                    flag = true;
                }
            }
            if (flag)
            {
                this._AppConfig.License = config.License;
                this._AppConfig.Save();
            }
            LicenseManager.Init(this._AppConfig);
            return flag;
        }

        private bool Start() => 
            LicenseManager.Start();

        void IDisposable.Dispose()
        {
            this.End();
        }

        public bool IsValid { get; private set; }
    }
}

