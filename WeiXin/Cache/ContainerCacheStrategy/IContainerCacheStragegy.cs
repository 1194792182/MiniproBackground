using System;
using System.Collections.Generic;
using System.Text;
using WeiXin.Cache.CacheStrategy;
using WeiXin.Containers;

namespace WeiXin.Cache.ContainerCacheStrategy
{
    /// <summary>
    /// 容器缓存策略接口
    /// </summary>
    public interface IContainerCacheStrategy : IBaseCacheStrategy<string, IBaseContainerBag>
    {
        /// <summary>
        /// 获取所有ContainerBag
        /// </summary>
        /// <typeparam name="TBag"></typeparam>
        /// <returns></returns>
        IDictionary<string, TBag> GetAll<TBag>() where TBag : IBaseContainerBag;

        /// <summary>
        /// 更新ContainerBag
        /// </summary>
        /// <param name="key"></param>
        /// <param name="containerBag"></param>
        /// <param name="isFullKey">是否已经是完整的Key，如果不是，则会调用一次GetFinalKey()方法</param>
        void UpdateContainerBag(string key, IBaseContainerBag containerBag, bool isFullKey = false);
    }
}
