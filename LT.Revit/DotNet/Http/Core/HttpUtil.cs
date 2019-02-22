namespace DotNet.Http.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Security;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Text.RegularExpressions;

    public class HttpUtil
    {
        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) => 
            true;

        public static HttpResponseParameter Excute(HttpRequestParameter requestParameter)
        {
            HttpWebRequest webRequest = (HttpWebRequest) WebRequest.Create(new Uri(requestParameter.Url, UriKind.RelativeOrAbsolute));
            SetHeader(webRequest, requestParameter);
            SetCookie(webRequest, requestParameter);
            if (Regex.IsMatch(requestParameter.Url, "^https://"))
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(HttpUtil.CheckValidationResult);
            }
            SetParameter(webRequest, requestParameter);
            return SetResponse(webRequest, requestParameter);
        }

        private static void SetCookie(HttpWebRequest webRequest, HttpRequestParameter requestParameter)
        {
            webRequest.CookieContainer = new CookieContainer();
            if ((requestParameter.Cookie != null) && !string.IsNullOrEmpty(requestParameter.Cookie.CookieString))
            {
                webRequest.Headers[HttpRequestHeader.Cookie] = requestParameter.Cookie.CookieString;
            }
            if (((requestParameter.Cookie != null) && (requestParameter.Cookie.CookieCollection != null)) && (requestParameter.Cookie.CookieCollection.Count > 0))
            {
                webRequest.CookieContainer.Add(requestParameter.Cookie.CookieCollection);
            }
        }

        private static void SetHeader(HttpWebRequest webRequest, HttpRequestParameter requestParameter)
        {
            webRequest.Method = requestParameter.IsPost ? "POST" : "GET";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.Accept = "text/html, application/xhtml+xml, */*";
            webRequest.KeepAlive = true;
            webRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko/20100101 Firefox/22.0";
            webRequest.AllowAutoRedirect = true;
            webRequest.ProtocolVersion = HttpVersion.Version11;
        }

        private static void SetParameter(HttpWebRequest webRequest, HttpRequestParameter requestParameter)
        {
            if (((requestParameter.Parameters != null) && (requestParameter.Parameters.Count > 0)) && requestParameter.IsPost)
            {
                StringBuilder builder = new StringBuilder(string.Empty);
                foreach (KeyValuePair<string, string> pair in requestParameter.Parameters)
                {
                    builder.AppendFormat("{0}={1}&", pair.Key, pair.Value);
                }
                string s = builder.Remove(builder.Length - 1, 1).ToString();
                byte[] bytes = requestParameter.Encoding.GetBytes(s);
                webRequest.ContentLength = bytes.Length;
                using (Stream stream = webRequest.GetRequestStream())
                {
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Close();
                }
            }
        }

        private static HttpResponseParameter SetResponse(HttpWebRequest webRequest, HttpRequestParameter requestParameter)
        {
            HttpResponseParameter parameter = new HttpResponseParameter();
            using (HttpWebResponse response = (HttpWebResponse) webRequest.GetResponse())
            {
                parameter.Uri = response.ResponseUri;
                parameter.StatusCode = response.StatusCode;
                HttpCookieType type = new HttpCookieType {
                    CookieCollection = response.Cookies,
                    CookieString = response.Headers["Set-Cookie"]
                };
                parameter.Cookie = type;
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), requestParameter.Encoding))
                {
                    parameter.Body = reader.ReadToEnd();
                }
            }
            return parameter;
        }
    }
}

