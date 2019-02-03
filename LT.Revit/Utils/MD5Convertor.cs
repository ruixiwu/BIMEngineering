using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web.Security;

namespace Utils
{
    public class MD5Convertor
    {
        private byte[] Bytes = new byte[1];
        private readonly string Key = "CADI";
        private readonly MD5 MD5Operator = MD5.Create();

        public string Decrypt(string targetValue)
        {
            if (string.IsNullOrEmpty(targetValue))
            {
                return string.Empty;
            }
            var provider = new DESCryptoServiceProvider();
            var num = targetValue.Length/2;
            var buffer = new byte[num];
            for (var i = 0; i < num; i++)
            {
                var num3 = Convert.ToInt32(targetValue.Substring(i*2, 2), 0x10);
                buffer[i] = (byte) num3;
            }
            provider.Key =
                Encoding.ASCII.GetBytes(
                    FormsAuthentication.HashPasswordForStoringInConfigFile(
                        FormsAuthentication.HashPasswordForStoringInConfigFile(Key, "md5").Substring(0, 8), "sha1")
                        .Substring(0, 8));
            provider.IV =
                Encoding.ASCII.GetBytes(
                    FormsAuthentication.HashPasswordForStoringInConfigFile(
                        FormsAuthentication.HashPasswordForStoringInConfigFile(Key, "md5").Substring(0, 8), "md5")
                        .Substring(0, 8));
            var stream = new MemoryStream();
            var stream2 = new CryptoStream(stream, provider.CreateDecryptor(), CryptoStreamMode.Write);
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
            var builder = new StringBuilder();
            var provider = new DESCryptoServiceProvider();
            var bytes = Encoding.Default.GetBytes(targetValue);
            provider.Key =
                Encoding.ASCII.GetBytes(
                    FormsAuthentication.HashPasswordForStoringInConfigFile(
                        FormsAuthentication.HashPasswordForStoringInConfigFile(Key, "md5").Substring(0, 8), "sha1")
                        .Substring(0, 8));
            provider.IV =
                Encoding.ASCII.GetBytes(
                    FormsAuthentication.HashPasswordForStoringInConfigFile(
                        FormsAuthentication.HashPasswordForStoringInConfigFile(Key, "md5").Substring(0, 8), "md5")
                        .Substring(0, 8));
            var stream = new MemoryStream();
            var stream2 = new CryptoStream(stream, provider.CreateEncryptor(), CryptoStreamMode.Write);
            stream2.Write(bytes, 0, bytes.Length);
            stream2.FlushFinalBlock();
            foreach (var num in stream.ToArray())
            {
                builder.AppendFormat("{0:X2}", num);
            }
            return builder.ToString();
        }

        public string MD5Encrypt32(string plain)
        {
            var str = string.Empty;
            Bytes = MD5Operator.ComputeHash(Encoding.UTF8.GetBytes(plain));
            for (var i = 0; i < Bytes.Length; i++)
            {
                str = str + Bytes[i].ToString("x2").ToLower();
            }
            return str;
        }
    }
}