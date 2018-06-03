﻿using System;
using System.Collections.Generic;
using System.Text;

namespace WeiXin.MessageQueue
{
    /// <summary>
    /// MyMessageQueue消息队列项
    /// </summary>
    public class MessageQueueItem
    {
        /// <summary>
        /// 队列项唯一标识
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 队列项目命中触发时执行的委托
        /// </summary>
        public Action Action { get; set; }
        /// <summary>
        /// 此实例对象的创建时间
        /// </summary>
        public DateTime AddTime { get; set; }
        /// <summary>
        /// 项目说明（主要用于调试）
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 初始化MyMessageQueue消息队列项
        /// </summary>
        /// <param name="key"></param>
        /// <param name="action"></param>
        /// <param name="description"></param>
        public MessageQueueItem(string key, Action action, string description = null)
        {
            Key = key;
            Action = action;
            Description = description;
            AddTime = DateTime.Now;
        }
    }
}
