﻿using System;
using System.Collections.Generic;
using System.Text;
using WeiXin.MessageQueue;

namespace WeiXin.Utilities.CacheUtility
{
    /// <summary>
    /// 缓存立即生效方法
    /// </summary>
    public class FlushCache : IDisposable
    {
        /// <summary>
        /// 是否立即个更新到缓存
        /// </summary>
        public bool DoFlush { get; set; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="doFlush">是否立即更新到缓存</param>
        public FlushCache(bool doFlush = true)
        {
            DoFlush = doFlush;
        }

        /// <summary>
        /// 释放，开始立即更新所有缓存
        /// </summary>
        public void Dispose()
        {
            if (DoFlush)
            {
                MyMessageQueue.OperateQueue();
            }
        }

        /// <summary>
        /// 创建一个FlushCache实例
        /// </summary>
        /// <param name="doFlush">是否立即更新到缓存</param>
        /// <returns></returns>
        public static FlushCache CreateInstance(bool doFlush = true)
        {
            return new FlushCache(doFlush);
        }
    }
}
