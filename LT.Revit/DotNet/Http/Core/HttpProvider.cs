namespace DotNet.Http.Core
{
    using System;

    public class HttpProvider : IHttpProvider
    {
        public HttpResponseParameter Excute(HttpRequestParameter requestParameter) => 
            HttpUtil.Excute(requestParameter);
    }
}

