using System;
using System.Net;

namespace DotNet.Http.Core
{
    public class HttpResponseParameter
    {
        public HttpResponseParameter()
        {
            Cookie = new HttpCookieType();
        }

        public string Body { get; set; }

        public HttpCookieType Cookie { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public Uri Uri { get; set; }
    }
}