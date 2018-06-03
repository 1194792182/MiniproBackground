using System;
using System.Collections.Generic;
using System.Text;

namespace WeiXin.Exceptions
{
    /// <summary>
    /// 微信自定义异常基类
    /// </summary>
    public class WeixinException : Exception
    {
        /// <summary>
        /// 当前正在请求的公众号AccessToken或AppId
        /// </summary>
        public string AccessTokenOrAppId { get; set; }

        /// <summary>
        /// WeixinException
        /// </summary>
        /// <param name="message">异常消息</param>
        /// <param name="logged">是否已经使用WeixinTrace记录日志，如果没有，WeixinException会进行概要记录</param>
        public WeixinException(string message, bool logged = false)
            : this(message, null, logged)
        {
        }

        /// <summary>
        /// WeixinException
        /// </summary>
        /// <param name="message">异常消息</param>
        /// <param name="inner">内部异常信息</param>
        /// <param name="logged">是否已经使用WeixinTrace记录日志，如果没有，WeixinException会进行概要记录</param>
        public WeixinException(string message, Exception inner, bool logged = false)
            : base(message, inner)
        {
            if (!logged)
            {
                //WeixinTrace.Log(string.Format("WeixinException（{0}）：{1}", this.GetType().Name, message));
                //WeixinTrace.WeixinExceptionLog(this);
            }
        }
    }
}
