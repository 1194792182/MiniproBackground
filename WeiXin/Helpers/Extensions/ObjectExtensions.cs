using System;
using System.Collections.Generic;
using System.Text;

namespace WeiXin.Helpers.Extensions
{
    /// <summary>
    /// 扩展方法
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// 把数据转换为Json格式（使用Newtonsoft.Json.dll）
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns></returns>
        public static string ToJson(this object data)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(data);
        }

        /// <summary>
        /// string.IsNullOrWhiteSpace()的扩展方法
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        /// <summary>
        /// string.IsNullOrEmpty()的扩展方法
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        /// <summary>
        /// string.Format()的扩展方法
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string FormatWith(this string format, params object[] args)
        {
            return string.Format(format, args);
        }
    }
}
