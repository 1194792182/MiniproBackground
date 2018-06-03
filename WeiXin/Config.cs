using System;
using System.Collections.Generic;
using System.Text;

namespace WeiXin
{
    public static class Config
    {
        /// <summary>
        /// 请求超时设置（以毫秒为单位），默认为10秒。
        /// 说明：此处常量专为提供给方法的参数的默认值，不是方法内所有请求的默认超时时间。
        /// </summary>
        public const int TIME_OUT = 10000;

        public static string ApiMpHost { get; set; } = "https://api.weixin.qq.com";

        /// <summary>
        /// 默认缓存键的第一级命名空间，默认值：DefaultCache
        /// </summary>
        public static string DefaultCacheNamespace = "DefaultCache";//TODO:需要考虑分布式的情况，后期需要储存在缓存中,或进行全局配置
    }
}
