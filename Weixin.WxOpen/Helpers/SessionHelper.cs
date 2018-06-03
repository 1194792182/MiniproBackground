using System;
using System.Collections.Generic;
using System.Text;

namespace Weixin.WxOpen.Helpers
{
    /// <summary>
    /// Session帮助类
    /// </summary>
    public class SessionHelper
    {
        /// <summary>
        /// 获取新的3rdSession名称
        /// </summary>
        /// <param name="bSize">Session名称长度，单位：B，建议为16的倍数，通常情况下16B已经够用（32位GUID字符串）</param>
        /// <returns></returns>
        public static string GetNewThirdSessionName(int bSize = 16)
        {
            string key = null;
            for (int i = 0; i < bSize / 16; i++)
            {
                key += Guid.NewGuid().ToString("n");
            }
            return key;
        }
    }
}
