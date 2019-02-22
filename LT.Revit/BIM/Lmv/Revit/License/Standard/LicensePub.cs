namespace BIM.Lmv.Revit.License.Standard
{
    using System;
    using System.Net;

    internal class LicensePub
    {
        private uint _bitFeatureId;
        private string _errorDescribe = "";
        private uint _featureId;
        private string _host = "";
        private bool _isLogin;
        private int _status = -1;
        private uint _timeout = 5;
        private string _xmlScope;
        private BitAnswer bitAnswerLock;

        private bool BitAnswerLogin(uint featureId, ref string host)
        {
            bool flag = false;
            this.bitAnswerLock = new BitAnswer();
            flag = this.BitAnswerLoginEx(featureId, host);
            if (!flag)
            {
                flag = this.BitAnswerRegister(featureId, ref host);
                if (flag)
                {
                    flag = this.BitAnswerLoginEx(featureId, host);
                }
            }
            return flag;
        }

        private bool BitAnswerLoginEx(uint featureId, string host)
        {
            bool flag = false;
            LoginMode remote = LoginMode.Remote;
            if (host == "")
            {
                remote = LoginMode.Auto;
            }
            else if (host.ToLower() == "localhost")
            {
                remote = LoginMode.Local;
            }
            else
            {
                IPAddress address;
                if (IPAddress.TryParse(host, out address))
                {
                    host = address.ToString();
                    this.bitAnswerLock.SetLocalServer(host, 0x2051, 10);
                }
                else
                {
                    host = "";
                }
            }
            flag = this.bitAnswerLock.LoginEx(null, null, featureId, this._xmlScope, remote);
            this._errorDescribe = this.bitAnswerLock.ErrorDescribe;
            return flag;
        }

        private bool BitAnswerLogout()
        {
            bool flag = false;
            flag = this.bitAnswerLock.Logout();
            this._errorDescribe = this.bitAnswerLock.ErrorDescribe;
            return flag;
        }

        private bool BitAnswerRegister(uint featureId, ref string host)
        {
            bool bSucess = false;
            formBitAnswerRegister register = new formBitAnswerRegister {
                featureId = featureId,
                strHost = host
            };
            register.ShowDialog();
            if (!register.bCancel)
            {
                bSucess = register.bSucess;
                this._host = register.strHost;
                host = register.strHost;
            }
            register.Close();
            register.Dispose();
            register = null;
            return bSucess;
        }

        public bool CheckSession()
        {
            bool flag = false;
            try
            {
                flag = this.BitAnswerLogout();
                flag = this.BitAnswerLoginEx(this._bitFeatureId, this._host);
                this._isLogin = flag;
                return flag;
            }
            catch (Exception)
            {
                this._errorDescribe = "在线检测失败，请检查网络！";
                return flag;
            }
        }

        public bool LoginEx(uint featureId, ref string host)
        {
            bool flag = false;
            try
            {
                this._featureId = featureId;
                this._bitFeatureId = featureId;
                this._host = host;
                flag = this.BitAnswerLogin(this._bitFeatureId, ref host);
                this._isLogin = flag;
                return flag;
            }
            catch (Exception)
            {
                this._errorDescribe = "产品登录失败，请检查网络！";
                return flag;
            }
        }

        public bool Logout()
        {
            bool flag = false;
            try
            {
                flag = this.BitAnswerLogout();
                this._isLogin = false;
                return flag;
            }
            catch (Exception)
            {
                this._errorDescribe = "退出登录失败，请检查网络！";
                return flag;
            }
        }

        public uint BitFeatureId =>
            this._bitFeatureId;

        public string ErrorDescribe =>
            this._errorDescribe;

        public uint FeatureId =>
            this._featureId;

        public string Host =>
            this._host;

        public bool IsLogin =>
            this._isLogin;

        public int Status =>
            this._status;

        public uint Timeout
        {
            get => 
                this._timeout;
            set
            {
                this._timeout = value;
            }
        }
    }
}

