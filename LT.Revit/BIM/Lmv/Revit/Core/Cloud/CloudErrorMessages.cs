using System.Collections.Generic;

namespace BIM.Lmv.Revit.Core.Cloud
{
    internal class CloudErrorMessages
    {
        private static readonly Dictionary<int, string> ErrorInfos = new Dictionary<int, string>();

        static CloudErrorMessages()
        {
            ErrorInfos.Add(0x3e8, "用户名密码错误！");
            ErrorInfos.Add(0x3e9, "用户未激活！");
            ErrorInfos.Add(0x3ea, "凭据验证失败！");
            ErrorInfos.Add(0x3eb, "邮箱格式不正确！");
            ErrorInfos.Add(0x3ec, "邮件发送失败！");
            ErrorInfos.Add(0x3ed, "激活失败！");
            ErrorInfos.Add(0x3ee, "密码格式不正确！");
            ErrorInfos.Add(0xbb8, "激活码不正确！");
            ErrorInfos.Add(0x1388, "程序异常！");
        }

        public static string GetErrorMsg(int errorCode)
        {
            string str;
            if (ErrorInfos.TryGetValue(errorCode, out str))
            {
                return str;
            }
            return string.Empty;
        }
    }
}