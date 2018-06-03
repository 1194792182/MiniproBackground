using System;
using System.Collections.Generic;
using System.Text;
using WeiXin.Cache.CacheStrategy;
using WeiXin.Cache.ContainerCacheStrategy;
using WeiXin.Cache.Local.ContainerCacheStrategy;
using WeiXin.Cache.Local.Lock;
using WeiXin.Cache.Lock;
using WeiXin.Cache.ObjectCacheStrategy;

namespace WeiXin.Cache.Local.ObjectCacheStrategy
{
    /// <summary>
    /// 全局静态数据源帮助类
    /// </summary>
    public static class LocalObjectCacheHelper
    {
        /// <summary>
        /// 所有数据集合的列表
        /// </summary>
        internal static IDictionary<string, object> LocalObjectCache { get; set; }

        static LocalObjectCacheHelper()
        {
            LocalObjectCache = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }
    }

    /// <summary>
    /// 本地容器缓存策略
    /// </summary>
    public class LocalObjectCacheStrategy : BaseCacheStrategy, IObjectCacheStrategy
    //where TContainerBag : class, IBaseContainerBag, new()
    {
        #region 数据源

        private IDictionary<string, object> _cache = LocalObjectCacheHelper.LocalObjectCache;

        #endregion

        #region 单例

        //静态LocalCacheStrategy
        public static LocalObjectCacheStrategy Instance
        {
            get
            {
                return Nested.instance;//返回Nested类中的静态成员instance
            }
        }

        class Nested
        {
            static Nested()
            {
            }
            //将instance设为一个初始化的LocalCacheStrategy新实例
            internal static readonly LocalObjectCacheStrategy instance = new LocalObjectCacheStrategy();
        }


        #endregion

        #region IObjectCacheStrategy 成员

        public IContainerCacheStrategy ContainerCacheStrategy
        {
            get { return LocalContainerCacheStrategy.Instance; }
        }

        public void InsertToCache(string key, object value)
        {
            if (key == null || value == null)
            {
                return;
            }

            var finalKey = base.GetFinalKey(key);

            _cache[finalKey] = value;
        }

        public void RemoveFromCache(string key, bool isFullKey = false)
        {
            var cacheKey = GetFinalKey(key, isFullKey);
            _cache.Remove(cacheKey);
        }

        public object Get(string key, bool isFullKey = false)
        {
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }

            if (!CheckExisted(key, isFullKey))
            {
                return null;
            }

            var cacheKey = GetFinalKey(key, isFullKey);
            return _cache[cacheKey];
        }

        public IDictionary<string, object> GetAll()
        {
            return _cache;
        }

        public bool CheckExisted(string key, bool isFullKey = false)
        {
            var cacheKey = GetFinalKey(key, isFullKey);
            return _cache.ContainsKey(cacheKey);
        }

        public long GetCount()
        {
            return _cache.Count;
        }

        public void Update(string key, object value, bool isFullKey = false)
        {
            var cacheKey = GetFinalKey(key, isFullKey);
            _cache[cacheKey] = value;
        }

        public void UpdateContainerBag(string key, object bag, bool isFullKey = false)
        {
            Update(key, bag, isFullKey);
        }

        #endregion

        #region ICacheLock
        public override ICacheLock BeginCacheLock(string resourceName, string key, int retryCount = 0, TimeSpan retryDelay = new TimeSpan())
        {
            return new LocalCacheLock(this, resourceName, key, retryCount, retryDelay);
        }

        #endregion

    }
}
