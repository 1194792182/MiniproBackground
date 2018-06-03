using System;
using System.Collections.Generic;
using System.Text;
using WeiXin.Entities.JsonResult.Interface;

namespace WeiXin.Entities.JsonResult
{
    /// <summary>
    /// 包含 errorcode 的 Json 返回结果接口
    /// </summary>
    public interface IWxJsonResult : IJsonResult
    {
        /// <summary>
        /// 返回结果代码
        /// </summary>
        ReturnCode errcode { get; set; }
    }

    /// <summary>
    /// 公众号 JSON 返回结果（用于菜单接口等）
    /// </summary>
    [Serializable]
    public class WxJsonResult : BaseJsonResult
    {
        public ReturnCode errcode { get; set; }

        /// <summary>
        /// 返回消息代码数字（同errcode枚举值）
        /// </summary>
        public override int ErrorCodeValue { get { return (int)errcode; } }


        public override string ToString()
        {
            return string.Format("WxJsonResult：{{errcode:'{0}',errcode_name:'{1}',errmsg:'{2}'}}",
                (int)errcode, errcode.ToString(), errmsg);
        }
    }
}
