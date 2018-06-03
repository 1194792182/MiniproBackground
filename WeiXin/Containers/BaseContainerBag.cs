using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using WeiXin.Cache;
using WeiXin.Entities;
using WeiXin.Helpers.Containers;
using WeiXin.MessageQueue;

namespace WeiXin.Containers
{
    /// <summary>
    /// IBaseContainerBag，BaseContainer容器中的Value类型
    /// </summary>
    public interface IBaseContainerBag
    {
        /// <summary>
        /// 用于标记，方便后台管理
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// 缓存键，形如：wx669ef95216eef885，最底层的Key，不考虑命名空间等
        /// </summary>
        string Key { get; set; }
        /// <summary>
        /// 当前对象被缓存的时间
        /// </summary>
        DateTime CacheTime { get; set; }
    }

    /// <summary>
    /// 提供给具有 AppId 的 IBaseContainerBag 使用的接口
    /// </summary>
    public interface IBaseContainerBag_AppId
    {
        /// <summary>
        /// AppId
        /// </summary>
        string AppId { get; set; }
    }

    /// <summary>
    /// BaseContainer容器中的Value类型
    /// </summary>
    [Serializable]
    public class BaseContainerBag : BindableBase, IBaseContainerBag
    {
        private string _key;
        private string _name;

        /// <summary>
        /// 用于标记，方便后台管理
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { this.SetContainerProperty(ref _name, value); }
        }

        /// <summary>
        /// 通常为AppId
        /// </summary>
        public string Key
        {
            get { return _key; }
            set { this.SetContainerProperty(ref _key, value); }
        }

        /// <summary>
        /// 缓存时间，不使用属性变化监听
        /// </summary>
        public DateTime CacheTime { get; set; }


        private void BaseContainerBag_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var containerBag = (IBaseContainerBag)sender;
            var mqKey = MyMessageQueue.GenerateKey("ContainerBag", sender.GetType(), containerBag.Key, "UpdateContainerBag");

            //获取对应Container的缓存相关

            //加入消息队列，每过一段时间进行自动更新，防止属性连续被编辑，短时间内反复更新缓存。
            var mq = new MyMessageQueue();
            mq.Add(mqKey, () =>
            {
                //var containerCacheStrategy = CacheStrategyFactory.GetContainerCacheStrategyInstance();
                var containerCacheStrategy = CacheStrategyFactory.GetObjectCacheStrategyInstance().ContainerCacheStrategy;
                var itemCacheKey = ContainerHelper.GetItemCacheKey(containerBag);
                containerBag.CacheTime = DateTime.Now;//记录缓存时间

                //cacheKey形如:Container:My.Weixin.MP.Containers.AccessTokenBag:wx669ef95216eef885
                containerCacheStrategy.UpdateContainerBag(itemCacheKey, containerBag);
            });
        }


        /// <summary>
        /// 设置Container属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="storage"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected bool SetContainerProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
        {
            var result = base.SetProperty(ref storage, value, propertyName);
            return result;
        }

        public BaseContainerBag()
        {
            base.PropertyChanged += BaseContainerBag_PropertyChanged;
        }
    }
}
