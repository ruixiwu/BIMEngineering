namespace BIM.Lmv.Revit.Core.Cloud
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Threading;

    internal class OssHelper
    {
        public static bool Post(Uri uri, Dictionary<string, string> headers, Dictionary<string, string> fields, WebProxy proxy, string key, Stream dataStream)
        {
            bool flag;
            string str = "----" + DateTime.Now.Ticks.ToString("x");
            string s = "--" + str + "\r\nContent-Disposition: form-data; name=\"key\"\r\n\r\n" + key + "\r\n";
            if ((fields != null) && (fields.Count > 0))
            {
                foreach (KeyValuePair<string, string> pair in fields)
                {
                    string str5 = s;
                    s = str5 + "--" + str + "\r\nContent-Disposition: form-data; name=\"" + pair.Key + "\"\r\n\r\n" + pair.Value + "\r\n";
                }
            }
            s = s + "--" + str + "\r\nContent-Disposition: form-data; name=\"file\"; filename=\"f\"\r\nContent-Type: application/octet-stream\r\n\r\n";
            string str3 = "\r\n--" + str + "--\r\n";
            byte[] bytes = Encoding.UTF8.GetBytes(s);
            byte[] buffer = Encoding.UTF8.GetBytes(str3);
            HttpWebRequest state = (HttpWebRequest) WebRequest.Create(uri);
            state.Referer = new Uri(uri, "/").ToString();
            state.ContentType = "multipart/form-data; boundary=" + str;
            state.Method = "POST";
            state.KeepAlive = true;
            state.Credentials = CredentialCache.DefaultCredentials;
            if (proxy != null)
            {
                state.Proxy = proxy;
            }
            state.ServicePoint.Expect100Continue = false;
            state.Timeout = -1;
            state.ReadWriteTimeout = 0x4e20;
            if ((headers != null) && (headers.Count > 0))
            {
                foreach (string str4 in headers.Keys)
                {
                    state.Headers[str4] = headers[str4];
                }
            }
            try
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    stream.Write(bytes, 0, bytes.Length);
                    dataStream.CopyTo(stream);
                    stream.Write(buffer, 0, buffer.Length);
                    state.ContentLength = stream.Length;
                    using (Stream stream2 = state.GetRequestStream())
                    {
                        stream.Position = 0L;
                        stream.WriteTo(stream2);
                    }
                }
                IAsyncResult asyncResult = state.BeginGetResponse(delegate (IAsyncResult x) {
                }, state);
                using (HttpWebResponse response = (HttpWebResponse) state.EndGetResponse(asyncResult))
                {
                    flag = response.StatusCode == HttpStatusCode.NoContent;
                }
            }
            catch (Exception)
            {
                state.Abort();
                flag = false;
            }
            return flag;
        }

        public static bool Post(Uri uri, Dictionary<string, string> headers, Dictionary<string, string> fields, WebProxy proxy, string key, Stream dataStream, int dataSize, Action<long> progressCallback, EventWaitHandle cancelEvent)
        {
            bool flag;
            string str = "----" + DateTime.Now.Ticks.ToString("x");
            string s = "--" + str + "\r\nContent-Disposition: form-data; name=\"key\"\r\n\r\n" + key + "\r\n";
            if ((fields != null) && (fields.Count > 0))
            {
                foreach (KeyValuePair<string, string> pair in fields)
                {
                    string str5 = s;
                    s = str5 + "--" + str + "\r\nContent-Disposition: form-data; name=\"" + pair.Key + "\"\r\n\r\n" + pair.Value + "\r\n";
                }
            }
            s = s + "--" + str + "\r\nContent-Disposition: form-data; name=\"file\"; filename=\"f\"\r\nContent-Type: application/octet-stream\r\n\r\n";
            string str3 = "\r\n--" + str + "--\r\n";
            byte[] bytes = Encoding.UTF8.GetBytes(s);
            byte[] buffer = Encoding.UTF8.GetBytes(str3);
            HttpWebRequest state = (HttpWebRequest) WebRequest.Create(uri);
            state.ContentType = "multipart/form-data; boundary=" + str;
            state.Method = "POST";
            state.KeepAlive = true;
            state.AllowWriteStreamBuffering = false;
            state.Credentials = CredentialCache.DefaultCredentials;
            if (proxy != null)
            {
                state.Proxy = proxy;
            }
            state.ServicePoint.Expect100Continue = false;
            state.Timeout = -1;
            state.ReadWriteTimeout = 0x4e20;
            state.ContentLength = (bytes.Length + dataSize) + buffer.Length;
            if ((headers != null) && (headers.Count > 0))
            {
                foreach (string str4 in headers.Keys)
                {
                    state.Headers[str4] = headers[str4];
                }
            }
            if ((cancelEvent != null) && cancelEvent.WaitOne(0))
            {
                return false;
            }
            try
            {
                using (Stream stream = state.GetRequestStream())
                {
                    int num2;
                    stream.Write(bytes, 0, bytes.Length);
                    byte[] buffer3 = new byte[0x8000];
                    long num = 0L;
                    while ((num2 = dataStream.Read(buffer3, 0, buffer3.Length)) > 0)
                    {
                        if ((cancelEvent != null) && cancelEvent.WaitOne(0))
                        {
                            return false;
                        }
                        stream.Write(buffer3, 0, num2);
                        num += num2;
                        if (progressCallback != null)
                        {
                            progressCallback(num);
                        }
                    }
                    stream.Write(buffer, 0, buffer.Length);
                }
                IAsyncResult asyncResult = state.BeginGetResponse(delegate (IAsyncResult x) {
                }, state);
                using (HttpWebResponse response = (HttpWebResponse) state.EndGetResponse(asyncResult))
                {
                    flag = response.StatusCode == HttpStatusCode.NoContent;
                }
            }
            catch (Exception)
            {
                state.Abort();
                flag = false;
            }
            return flag;
        }
    }
}

