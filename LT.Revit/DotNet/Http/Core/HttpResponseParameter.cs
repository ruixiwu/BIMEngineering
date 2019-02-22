namespace DotNet.Http.Core
{
    using System;
    using System.Net;
    using System.Runtime.CompilerServices;

    public class HttpResponseParameter
    {
        public HttpResponseParameter()
        {
            this.Cookie = new HttpCookieType();
        }

        public string Body { get; set; }

        public HttpCookieType Cookie { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public System.Uri Uri { get; set; }
    }
}

