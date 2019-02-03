using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace Utils
{
    public class WebServiceClient
    {
        public string CHECKUSERLOGIN = "checkUserLogin";
        public string GET14CODETYPE = "getComTypeCodeTree";
        public string GET90CODETYPE = "getComTypeTree";
        public string getModeType = "getModeType";
        public string getPrjInfo = "getPrjInfo";
        public string GETUSERIDBYPHONE = "getUsrIdByPhone";
        private readonly string HTTPHEADER = "http://47.94.91.246/Liems/webservice/";
        public string UPLOADCOMPONENT = "upLoadComponent";
        private readonly string WADL = "?_wadl";

        public string Post(string strType, Dictionary<string, string> dictParam)
        {
            var requestUri = new Uri(HTTPHEADER + strType + WADL);
            var request = WebRequest.Create(requestUri) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            var builder = new StringBuilder();
            foreach (var pair in dictParam)
            {
                var str = string.Format("&{0}={1}", pair.Key, HttpUtility.UrlEncode(pair.Value));
                builder.Append(str);
            }
            var bytes = Encoding.UTF8.GetBytes(builder.ToString());
            request.ContentLength = bytes.Length;
            using (var stream = request.GetRequestStream())
            {
                stream.Write(bytes, 0, bytes.Length);
            }
            using (var response = request.GetResponse() as HttpWebResponse)
            {
                var reader = new StreamReader(response.GetResponseStream());
                return reader.ReadToEnd();
            }
        }

        public string Post(FamilyParamHeaderName name, string value, ServiceInterfaceEnum servInterface)
        {
            var str = "http://47.94.91.246/Liems/webservice/";
            var str2 = "?_wadl";
            var uriString = str + servInterface + str2;
            var dictionary = new Dictionary<string, string>();
            dictionary[name.ToString()] = value;
            var requestUri = new Uri(uriString);
            var request = WebRequest.Create(requestUri) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            var builder = new StringBuilder();
            foreach (var pair in dictionary)
            {
                var str4 = string.Format("&{0}={1}", pair.Key, HttpUtility.UrlEncode(pair.Value));
                builder.Append(str4);
            }
            var bytes = Encoding.UTF8.GetBytes(builder.ToString());
            request.ContentLength = bytes.Length;
            using (var stream = request.GetRequestStream())
            {
                stream.Write(bytes, 0, bytes.Length);
            }
            using (var response = request.GetResponse() as HttpWebResponse)
            {
                var reader = new StreamReader(response.GetResponseStream());
                return reader.ReadToEnd();
            }
        }
    }
}