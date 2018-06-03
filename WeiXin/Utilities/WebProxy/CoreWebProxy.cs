using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace WeiXin.Utilities.WebProxy
{
    /// <summary>
    /// .NET Core 使用的 WebProxy 类
    /// 参考：http://www.abelliu.com/dotnet/dotnet%20core/2017/03/14/dotnetcore-proxy/
    /// </summary>
    public class CoreWebProxy : IWebProxy
    {
        public readonly Uri Uri;
        public readonly string[] BypassList;

        /// <summary>
        /// WebProxy for .net core
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="credentials"></param>
        /// <param name="bypass"></param>
        public CoreWebProxy(Uri uri, ICredentials credentials = null, string[] bypassList = null)
        {
            Uri = uri;
            BypassList = bypassList;
            Credentials = credentials;
        }

        public ICredentials Credentials { get; set; }

        public Uri GetProxy(Uri destination) => Uri;

        public bool IsBypassed(Uri host) => BypassList?.Select(bypass => new Uri(bypass)).Contains(host) ?? false;

        public override int GetHashCode()
        {
            if (Uri == null)
            {
                return -1;
            }

            return Uri.GetHashCode();
        }
    }
}
