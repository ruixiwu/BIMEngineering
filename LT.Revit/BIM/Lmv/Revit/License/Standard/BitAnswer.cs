namespace BIM.Lmv.Revit.License.Standard
{
    using System;
    using System.IO;
    using System.Text;

    internal class BitAnswer
    {
        private string _errorDescribe = "";
        private uint _featureId;
        private string _host = "";
        private int _status = -1;
        private BitAnswerInterface bitAnswer;

        public BitAnswer()
        {
            if (this.IsX64)
            {
                this.bitAnswer = new BitAnswerX64();
            }
            else
            {
                this.bitAnswer = new BitAnswerX86();
            }
        }

        public string ApplyUpdateInfo(string updateInfo)
        {
            uint receiptInfoSize = 0x2800;
            byte[] receiptInfo = new byte[receiptInfoSize];
            this._status = this.bitAnswer.ApplyUpdateInfo(updateInfo, receiptInfo, ref receiptInfoSize);
            if (this._status == 260)
            {
                receiptInfo = new byte[receiptInfoSize];
                this._status = this.bitAnswer.ApplyUpdateInfo(updateInfo, receiptInfo, ref receiptInfoSize);
            }
            if (this._status != 0)
            {
                throw new BitAnswerException(this._status);
            }
            return Encoding.UTF8.GetString(receiptInfo);
        }

        public uint ConvertFeature(uint featureId, uint para1, uint para2, uint para3, uint para4)
        {
            uint result = 0;
            this._status = this.bitAnswer.ConvertFeature(featureId, para1, para2, para3, para4, ref result);
            if (this._status != 0)
            {
                throw new BitAnswerException(this._status);
            }
            return result;
        }

        public byte[] DecryptFeature(uint featureId, byte[] cipherBuffer)
        {
            byte[] plainBuffer = new byte[cipherBuffer.Length];
            this._status = this.bitAnswer.DecryptFeature(featureId, cipherBuffer, plainBuffer);
            if (this._status != 0)
            {
                throw new BitAnswerException(this._status);
            }
            return plainBuffer;
        }

        public byte[] EncryptFeature(uint featureId, byte[] plainBuffer)
        {
            byte[] cipherBuffer = new byte[plainBuffer.Length];
            this._status = this.bitAnswer.EncryptFeature(featureId, plainBuffer, cipherBuffer);
            if (this._status != 0)
            {
                throw new BitAnswerException(this._status);
            }
            return cipherBuffer;
        }

        public byte[] GetDataItem(string dataItemName)
        {
            uint dataItemValueLen = 0x400;
            byte[] dataItemValue = new byte[dataItemValueLen];
            this._status = this.bitAnswer.GetDataItem(dataItemName, dataItemValue, ref dataItemValueLen);
            if (this._status != 0)
            {
                throw new BitAnswerException(this._status);
            }
            byte[] destinationArray = new byte[dataItemValueLen];
            Array.Copy(dataItemValue, destinationArray, (long) dataItemValueLen);
            return destinationArray;
        }

        public string GetDataItemName(uint index)
        {
            uint nameLen = 0x81;
            byte[] name = new byte[nameLen];
            this._status = this.bitAnswer.GetDataItemName(index, name, ref nameLen);
            if (this._status != 0)
            {
                throw new BitAnswerException(this._status);
            }
            return Encoding.GetEncoding("gbk").GetString(name);
        }

        public uint GetDataItemNum()
        {
            uint number = 0;
            this._status = this.bitAnswer.GetDataItemNum(ref number);
            if (this._status != 0)
            {
                throw new BitAnswerException(this._status);
            }
            return number;
        }

        public string GetErrorDescribe(int status)
        {
            switch (status)
            {
                case 0:
                    return "BitAnswer Success";

                case 0x108:
                    return ("BitAnswer ErrorCode:" + status.ToString() + "\r\nDescribe:服务器没有响应，请确认服务器地址和端口配置正确");

                case 0x114:
                    return ("BitAnswer ErrorCode:" + status.ToString() + "\r\nDescribe:没有找到相应的本地授权许可数据文件");

                case 0x503:
                    return ("BitAnswer ErrorCode:" + status.ToString() + "\r\nDescribe:指定的特征项没有找到");

                case 0x702:
                    return ("BitAnswer ErrorCode:" + status.ToString() + "\r\nDescribe:在线用户数超过限制");

                case 0x11d:
                    return ("BitAnswer ErrorCode:" + status.ToString() + "\r\nDescribe:发现本地系统时间篡改。当前时间比最近一次使用时间还要早");

                case 0x123:
                    return ("BitAnswer ErrorCode:" + status.ToString() + "\r\nDescribe:授权码已经从本机迁出");

                case 0x705:
                    return ("BitAnswer ErrorCode:" + status.ToString() + "\r\nDescribe:授权码被禁用");

                case 0x70c:
                    return ("BitAnswer ErrorCode:" + status.ToString() + "\r\nDescribe:IP地址被禁用，请联系开发商");

                case 0x719:
                    return ("BitAnswer ErrorCode:" + status.ToString() + "\r\nDescribe:授权码转移的机器数量超过限制");

                case 0x803:
                    return ("BitAnswer ErrorCode:" + status.ToString() + "\r\nDescribe:下载次数超过限制");

                case 0x807:
                    return ("BitAnswer ErrorCode:" + status.ToString() + "\r\nDescribe:本地系统时间错误，请检查系统时间及时区设置");

                case 0x780:
                    return ("BitAnswer ErrorCode:" + status.ToString() + "\r\nDescribe:授权码的验证到期，为了正常使用，请再次认证以延长授权码使用期限");

                case 0x785:
                    return ("BitAnswer ErrorCode:" + status.ToString() + "\r\nDescribe:演示授权不支持升级操作");
            }
            return ("BitAnswer ErrorCode:" + status.ToString() + "\r\nDescribe:未知错误");
        }

        public string GetInfo(string sn, InfoType type)
        {
            uint infoSize = 0x2800;
            byte[] info = new byte[infoSize];
            this._status = this.bitAnswer.GetInfo(sn, (uint) type, info, ref infoSize);
            return Encoding.UTF8.GetString(info);
        }

        public string GetRequestInfo(string url, string sn, uint bindingType)
        {
            uint requestInfoSize = 0x2800;
            byte[] requestInfo = new byte[requestInfoSize];
            this._status = this.bitAnswer.GetRequestInfo(sn, bindingType, requestInfo, ref requestInfoSize);
            if (this._status == 260)
            {
                requestInfo = new byte[requestInfoSize];
                this._status = this.bitAnswer.GetRequestInfo(sn, bindingType, requestInfo, ref requestInfoSize);
            }
            if (this._status != 0)
            {
                throw new BitAnswerException(this._status);
            }
            return Encoding.UTF8.GetString(requestInfo);
        }

        public string GetSessionInfo(SessionType type)
        {
            uint sessionInfoLen = 0x2800;
            byte[] sessionInfo = new byte[sessionInfoLen];
            this._status = this.bitAnswer.GetSessionInfo(type, sessionInfo, ref sessionInfoLen);
            if (this._status == 260)
            {
                sessionInfo = new byte[sessionInfoLen];
                this._status = this.bitAnswer.GetSessionInfo(type, sessionInfo, ref sessionInfoLen);
            }
            if (this._status != 0)
            {
                throw new BitAnswerException(this._status);
            }
            return Encoding.UTF8.GetString(sessionInfo);
        }

        public string GetUpdateInfo(string url, string sn, string requestInfo)
        {
            uint updateInfoSize = 0x400;
            byte[] updateInfo = new byte[updateInfoSize];
            this._status = this.bitAnswer.GetUpdateInfo(url, sn, requestInfo, updateInfo, ref updateInfoSize);
            if (this._status == 260)
            {
                updateInfo = new byte[updateInfoSize];
                this._status = this.bitAnswer.GetUpdateInfo(url, sn, requestInfo, updateInfo, ref updateInfoSize);
            }
            if (this._status != 0)
            {
                throw new BitAnswerException(this._status);
            }
            return Encoding.UTF8.GetString(updateInfo);
        }

        public bool Login(string url, string sn, LoginMode mode)
        {
            this._status = this.bitAnswer.Login(url, sn, (int) mode);
            return (this._status == 0);
        }

        public bool LoginEx(string url, string sn, uint featureId, string xmlScope, LoginMode mode)
        {
            this._featureId = featureId;
            this._status = this.bitAnswer.LoginEx(url, sn, featureId, xmlScope, (int) mode);
            return (this._status == 0);
        }

        public bool Logout()
        {
            this._status = this.bitAnswer.Logout();
            return (this._status == 0);
        }

        public uint QueryFeature(uint featureId)
        {
            uint capacity = 0;
            this._status = this.bitAnswer.QueryFeature(featureId, ref capacity);
            if (this._status != 0)
            {
                throw new BitAnswerException(this._status);
            }
            return capacity;
        }

        public uint ReadFeature(uint featureId)
        {
            uint featureValue = 0;
            this._status = this.bitAnswer.ReadFeature(featureId, ref featureValue);
            if (this._status != 0)
            {
                throw new BitAnswerException(this._status);
            }
            return featureValue;
        }

        public uint ReleaseFeature(uint featureId)
        {
            uint capacity = 0;
            this._status = this.bitAnswer.ReleaseFeature(featureId, ref capacity);
            if (this._status != 0)
            {
                throw new BitAnswerException(this._status);
            }
            return capacity;
        }

        public bool RemoveDataItem(string dataItemName)
        {
            this._status = this.bitAnswer.RemoveDataItem(dataItemName);
            return (this._status == 0);
        }

        public string Revoke(string url, string sn)
        {
            uint revocationInfoSize = 0x2800;
            byte[] revocationInfo = new byte[revocationInfoSize];
            this._status = this.bitAnswer.Revoke(url, sn, revocationInfo, ref revocationInfoSize);
            if (this._status == 260)
            {
                revocationInfo = new byte[revocationInfoSize];
                this._status = this.bitAnswer.Revoke(url, sn, revocationInfo, ref revocationInfoSize);
            }
            if (this._status != 0)
            {
                throw new BitAnswerException(this._status);
            }
            return Encoding.UTF8.GetString(revocationInfo);
        }

        public bool RevokeOnline(string url, string sn)
        {
            uint revocationInfoSize = 0x2800;
            this._status = this.bitAnswer.Revoke(url, sn, null, ref revocationInfoSize);
            return (this._status == 0);
        }

        public bool SetDataItem(string dataItemName, byte[] dataItemValue)
        {
            this._status = this.bitAnswer.SetDataItem(dataItemName, dataItemValue);
            return (this._status == 0);
        }

        public bool SetDbPath(string path)
        {
            this._status = this.bitAnswer.SetDbPath(path);
            return (this._status == 0);
        }

        public bool SetLocalServer(string host, uint port, uint timeoutSeconds)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\BitAnswer\8F79-2-4EB";
            if (Directory.Exists(path))
            {
                File.Delete(path + @"\bit_config.xml");
            }
            this._status = this.bitAnswer.SetLocalServer(host, port, timeoutSeconds);
            return (this._status == 0);
        }

        public bool SetProxy(string host, uint port, string userId, string password)
        {
            this._status = this.bitAnswer.SetProxy(host, port, userId, password);
            return (this._status == 0);
        }

        public bool UpdateOnline(string url, string sn)
        {
            this._status = this.bitAnswer.UpdateOnline(url, sn);
            return (this._status == 0);
        }

        public bool WriteFeature(uint featureId, uint featureValue)
        {
            this._status = this.bitAnswer.WriteFeature(featureId, featureValue);
            return (this._status == 0);
        }

        public string ErrorDescribe
        {
            get
            {
                this._errorDescribe = this.GetErrorDescribe(this._status);
                return this._errorDescribe;
            }
        }

        public uint FeatureId =>
            this._featureId;

        public string Host =>
            this._host;

        public bool IsX64 =>
            (IntPtr.Size == 8);

        public int Status =>
            this._status;
    }
}

