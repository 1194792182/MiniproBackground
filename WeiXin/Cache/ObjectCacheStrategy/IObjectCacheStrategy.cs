using System;
using System.Collections.Generic;
using System.Text;
using WeiXin.Cache.CacheStrategy;
using WeiXin.Cache.ContainerCacheStrategy;

namespace WeiXin.Cache.ObjectCacheStrategy
{
    /// <summary>
    /// 所有以String类型为Key的缓存策略接口
    /// </summary>
    public interface IObjectCacheStrategy : IBaseCacheStrategy<string, object>
    {
        IContainerCacheStrategy ContainerCacheStrategy { get; }
    }
}
