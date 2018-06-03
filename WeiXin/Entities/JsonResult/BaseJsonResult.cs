using System;
using WeiXin.Entities.JsonResult.Interface;

namespace WeiXin.Entities.JsonResult
{
    [Serializable]
    public abstract class BaseJsonResult : IJsonResult
    {
        /// <summary>
        /// 返回结果信息
        /// </summary>
        public virtual string errmsg { get; set; }

        /// <summary>
        /// errcode的
        /// </summary>
        public abstract int ErrorCodeValue { get; }
        public virtual object P2PData { get; set; }
    }
}
