using System;
using System.Collections.Generic;
using System.Text;
using WeiXin.Entities.JsonResult;

namespace Weixin.WxOpen.AdvancedAPIs.Sns.SnsJson
{
    /// <summary>
    /// JsCode2Json接口结果
    /// </summary>
    public class JsCode2JsonResult : WxJsonResult
    {
        /// <summary>
        /// 用户唯一标识
        /// </summary>
        public string openid { get; set; }
        /// <summary>
        /// 会话密钥
        /// </summary>
        public string session_key { get; set; }
        /// <summary>
        /// 用户在开放平台的唯一标识符。本字段在满足一定条件的情况下才返回。具体参看：https://mp.weixin.qq.com/debug/wxadoc/dev/api/uinionID.html
        /// </summary>
        public string unionid { get; set; }
    }
}
