using System.Linq;

namespace BIM.Lmv.Revit.Core.Cloud
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Web;

    internal class HttpHelper
    {
        private static string BuildQueryString(Dictionary<string, string> fields)
        {
            if ((fields == null) || (fields.Count == 0))
            {
                return string.Empty;
            }
            return string.Join("&", (IEnumerable<string>) (from x in fields select HttpUtility.UrlEncode(x.Key) + "=" + HttpUtility.UrlEncode(x.Value)));
        }

        public static string HttpGet(string url, Dictionary<string, string> fields)
        {
            string str3;
            string str = BuildQueryString(fields);
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url + ((str == "") ? "" : "?") + str);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";
            using (HttpWebResponse response = (HttpWebResponse) request.GetResponse())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.GetEncoding("utf-8")))
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
            string str = BuildQueryString(fields);
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url + ((str == "") ? "" : "?") + str);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";
            using (HttpWebResponse response = (HttpWebResponse) request.GetResponse())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    MemoryStream destination = new MemoryStream();
                    stream.CopyTo(destination);
                    stream3 = destination;
                }
            }
            return stream3;
        }

        public static string HttpPost(string url, Dictionary<string, string> fields)
        {
            string str3;
            string s = BuildQueryString(fields);
            byte[] bytes = Encoding.ASCII.GetBytes(s);
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = bytes.Length;
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(bytes, 0, bytes.Length);
            }
            using (HttpWebResponse response = (HttpWebResponse) request.GetResponse())
            {
                using (Stream stream2 = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream2, Encoding.GetEncoding("utf-8")))
                    {
                        str3 = reader.ReadToEnd();
                    }
                }
            }
            return str3;
        }
    }
}

