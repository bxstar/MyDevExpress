using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Caching;

namespace TaoBaoDataServer.WinClientData
{
    /// <summary>
    /// 公共缓存
    /// </summary>
    public class MyCache
    {

        /// <summary>
        /// 调用淘宝API后，本地的缓存数据
        /// </summary>
        private static ObjectCache LocalCache = MemoryCache.Default;

        /// <summary>
        /// 本地缓存，写入
        /// </summary>
        public static void SetLocalCache(string key, object value)
        {
            CacheItemPolicy policy = new CacheItemPolicy();
            policy.Priority = CacheItemPriority.NotRemovable;
            LocalCache.Set(key, value, policy);
        }

        /// <summary>
        /// 本地缓存，获取
        /// </summary>
        public static object GetLocalCache(string key)
        {
            if (LocalCache.Contains(key))
            {
                return LocalCache.Get(key);
            }
            else
            {
                return null;
            }
        }
    }
}
