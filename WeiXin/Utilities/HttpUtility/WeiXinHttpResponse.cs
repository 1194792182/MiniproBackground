using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace WeiXin.Utilities.HttpUtility
{
    public class WeiXinHttpResponse
    {
        public HttpResponseMessage Result { get; set; }

        public WeiXinHttpResponse(HttpResponseMessage httpWebResponse)
        {
            Result = httpWebResponse;
        }
    }
}
