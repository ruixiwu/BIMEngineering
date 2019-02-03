using System.Collections.Generic;
using System.Text;

namespace DotNet.Http.Core
{
    public class HttpRequestParameter
    {
        public HttpRequestParameter()
        {
            Encoding = Encoding.UTF8;
        }

        public HttpCookieType Cookie { get; set; }

        public Encoding Encoding { get; set; }

        public bool IsPost { get; set; }

        public IDictionary<string, string> Parameters { get; set; }

        public string RefererUrl { get; set; }

        public string Url { get; set; }
    }
}