namespace Utils
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web.Security;

    public class MD5Convertor
    {
        private byte[] Bytes = new byte[1];
        private string Key = "CADI";
        private MD5 MD5Operator = MD5.Create();

        public string Decrypt(string targetValue)
        {
            if (string.IsNullOrEmpty(targetValue))
            {
                return string.Empty;
            }
            DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
            int num = targetValue.Length / 2;
            byte[] buffer = new byte[num];
            for (int i = 0; i < num; i++)
            {
                int num3 = Convert.ToInt32(targetValue.Substring(i * 2, 2), 0x10);
                buffer[i] = (byte) num3;
            }
            provider.Key = Encoding.ASCII.GetBytes(FormsAuthentication.HashPasswordForStoringInConfigFile(FormsAuthentication.HashPasswordForStoringInConfigFile(this.Key, "md5").Substring(0, 8), "sha1").Substring(0, 8));
            provider.IV = Encoding.ASCII.GetBytes(FormsAuthentication.HashPasswordForStoringInConfigFile(FormsAuthentication.HashPasswordForStoringInConfigFile(this.Key, "md5").Substring(0, 8), "md5").Substring(0, 8));
            MemoryStream stream = new MemoryStream();
            CryptoStream stream2 = new CryptoStream(stream, provider.CreateDecryptor(), CryptoStreamMode.Write);
            stream2.Write(buffer, 0, buffer.Length);
            stream2.FlushFinalBlock();
            return Encoding.Default.GetString(stream.ToArray());
        }

        public string Encrypt(string targetValue)
        {
            if (string.IsNullOrEmpty(targetValue))
            {
                return string.Empty;
            }
            StringBuilder builder = new StringBuilder();
            DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
            byte[] bytes = Encoding.Default.GetBytes(targetValue);
            provider.Key = Encoding.ASCII.GetBytes(FormsAuthentication.HashPasswordForStoringInConfigFile(FormsAuthentication.HashPasswordForStoringInConfigFile(this.Key, "md5").Substring(0, 8), "sha1").Substring(0, 8));
            provider.IV = Encoding.ASCII.GetBytes(FormsAuthentication.HashPasswordForStoringInConfigFile(FormsAuthentication.HashPasswordForStoringInConfigFile(this.Key, "md5").Substring(0, 8), "md5").Substring(0, 8));
            MemoryStream stream = new MemoryStream();
            CryptoStream stream2 = new CryptoStream(stream, provider.CreateEncryptor(), CryptoStreamMode.Write);
            stream2.Write(bytes, 0, bytes.Length);
            stream2.FlushFinalBlock();
            foreach (byte num in stream.ToArray())
            {
                builder.AppendFormat("{0:X2}", num);
            }
            return builder.ToString();
        }

        public string MD5Encrypt32(string plain)
        {
            string str = string.Empty;
            this.Bytes = this.MD5Operator.ComputeHash(Encoding.UTF8.GetBytes(plain));
            for (int i = 0; i < this.Bytes.Length; i++)
            {
                str = str + this.Bytes[i].ToString("x2").ToLower();
            }
            return str;
        }
    }
}

