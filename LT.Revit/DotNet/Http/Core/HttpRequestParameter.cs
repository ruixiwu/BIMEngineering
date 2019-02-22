namespace DotNet.Http.Core
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Text;

    public class HttpRequestParameter
    {
        public HttpRequestParameter()
        {
            this.Encoding = System.Text.Encoding.UTF8;
        }

        public HttpCookieType Cookie { get; set; }

        public System.Text.Encoding Encoding { get; set; }

        public bool IsPost { get; set; }

        public IDictionary<string, string> Parameters { get; set; }

        public string RefererUrl { get; set; }

        public string Url { get; set; }
    }
}

