using System.Collections.Generic;
using System.Text;
using DotNet.Http.Core;

namespace BIM.Lmv.Revit.Utility
{
    public class LKAPIHelper
    {
        private const string LKAPIUrl = "http://47.94.91.246/Liems/webservice/";

        public static string getData(string url, IDictionary<string, string> Parameters)
        {
            IHttpProvider provider = new HttpProvider();
            var requestParameter = new HttpRequestParameter
            {
                Url = "http://47.94.91.246/Liems/webservice/" + url,
                IsPost = false,
                Encoding = Encoding.UTF8,
                Parameters = Parameters
            };
            return provider.Excute(requestParameter).Body;
        }
    }
}