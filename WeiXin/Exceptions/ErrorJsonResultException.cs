using System;
using System.Collections.Generic;
using System.Text;
using WeiXin.Entities.JsonResult;

namespace WeiXin.Exceptions
{
    /// <summary>
    /// JSON返回错误代码异常（比如access_token相关操作中使用）
    /// </summary>
    public class ErrorJsonResultException : WeixinException
    {
        /// <summary>
        /// JsonResult
        /// </summary>
        public WxJsonResult JsonResult { get; set; }
        /// <summary>
        /// 接口 URL
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// ErrorJsonResultException
        /// </summary>
        /// <param name="message">异常消息</param>
        /// <param name="inner">内部异常</param>
        /// <param name="jsonResult">WxJsonResult</param>
        /// <param name="url">API地址</param>
        public ErrorJsonResultException(string message, Exception inner, WxJsonResult jsonResult, string url = null)
            : base(message, inner, true)
        {
            JsonResult = jsonResult;
            Url = url;

            //WeixinTrace.ErrorJsonResultExceptionLog(this);
        }
    }
}
