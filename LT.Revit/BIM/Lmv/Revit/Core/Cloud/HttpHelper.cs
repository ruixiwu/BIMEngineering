using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace BIM.Lmv.Revit.Core.Cloud
{
    internal class HttpHelper
    {
        private static string BuildQueryString(Dictionary<string, string> fields)
        {
            if ((fields == null) || (fields.Count == 0))
            {
                return string.Empty;
            }
            return string.Join("&",
                from x in fields select HttpUtility.UrlEncode(x.Key) + "=" + HttpUtility.UrlEncode(x.Value));
        }

        public static string HttpGet(string url, Dictionary<string, string> fields)
        {
            string str3;
            var str = BuildQueryString(fields);
            var request = (HttpWebRequest) WebRequest.Create(url + (str == "" ? "" : "?") + str);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";
            using (var response = (HttpWebResponse) request.GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    using (var reader = new StreamReader(stream, Encoding.GetEncoding("utf-8")))
                    {
                        str3 = reader.ReadToEnd();
                    }
                }
            }
            return str3;
        }

        public static MemoryStream HttpGetStream(string url, Dictionary<string, string> fields)
        {
            MemoryStream stream3;
            var str = BuildQueryString(fields);
            var request = (HttpWebRequest) WebRequest.Create(url + (str == "" ? "" : "?") + str);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";
            using (var response = (HttpWebResponse) request.GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    var destination = new MemoryStream();
                    stream.CopyTo(destination);
                    stream3 = destination;
                }
            }
            return stream3;
        }

        public static string HttpPost(string url, Dictionary<string, string> fields)
        {
            string str3;
            var s = BuildQueryString(fields);
            var bytes = Encoding.ASCII.GetBytes(s);
            var request = (HttpWebRequest) WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = bytes.Length;
            using (var stream = request.GetRequestStream())
            {
                stream.Write(bytes, 0, bytes.Length);
            }
            using (var response = (HttpWebResponse) request.GetResponse())
            {
                using (var stream2 = response.GetResponseStream())
                {
                    using (var reader = new StreamReader(stream2, Encoding.GetEncoding("utf-8")))
                    {
                        str3 = reader.ReadToEnd();
                    }
                }
            }
            return str3;
        }
    }
}