using System.Net.Http;

namespace Utils
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Web;

    public class WebServiceClient
    {
        public string CHECKUSERLOGIN = "checkUserLogin";
        public string GET14CODETYPE = "getComTypeCodeTree";
        public string GET90CODETYPE = "getComTypeTree";
        public string getModeType = "getModeType";
        public string getWrkInfo = "getWrkInfo";
        public string GETUSERIDBYPHONE = "getUsrIdByPhone";
        public static string HTTPHEADER = "";
        public string UPLOADCOMPONENT = "upLoadComponent";
        private string WADL = "?_wadl";

        public string Post(string strType, Dictionary<string, string> dictParam)
        {
            Uri requestUri = new Uri(HTTPHEADER + strType + this.WADL);
            HttpWebRequest request = WebRequest.Create(requestUri) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            StringBuilder builder = new StringBuilder();
            foreach (KeyValuePair<string, string> pair in dictParam)
            {
                string str = $"&{pair.Key}={HttpUtility.UrlEncode(pair.Value)}";
                builder.Append(str);
            }
            byte[] bytes = Encoding.UTF8.GetBytes(builder.ToString());
            request.ContentLength = bytes.Length;
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(bytes, 0, bytes.Length);
            }
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());
                return reader.ReadToEnd();

                //HttpResponseMessage result = new HttpResponseMessage
                //{
                //    Content = new StringContent(responseRes, Encoding.GetEncoding("UTF-8"), "application/json")
                //};//这里是去掉反斜杠再放回出去，json就只剩下双引号。
                //return result.ToString();//结果还是有很多双引号的，不知如何解决
            }

        }
        }
    }

