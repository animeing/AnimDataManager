using AnimDataManager.DataBase;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace AnimDataManager.AutoLoader
{
    public sealed class DataCacheStore
    {
        private DataCacheStore() { }
        public static DataCacheStore Instance { get; } = new DataCacheStore();
        private ConcurrentDictionary<Type, CacheBase> cacheData = new ConcurrentDictionary<Type, CacheBase>();

        public bool IsRegisted<T1, T2>()
        where T1 : DaoBase<T2>, new()
        where T2 : DtoBase<T2>, new()
        {
            return IsRegisted(typeof(T1));
        }

        public bool IsRegisted(Type type)
        {
            return cacheData.ContainsKey(type);
        }

        public bool Regist<T1, T2>()
        where T1 : DaoBase<T2>, new()
        where T2 : DtoBase<T2>, new()
        {
            Type daoKey = typeof(T1);
            if (IsRegisted(daoKey))
            {
                return false;
            }
            return cacheData.TryAdd(daoKey, new DataCache<T1, T2>());
        }

        public bool Erase<T1, T2>()
        where T1 : DaoBase<T2>, new()
        where T2 : DtoBase<T2>, new()
        {
            Type daoKey = typeof(T1);
            if (!IsRegisted(daoKey))
            {
                return false;
            }
            CacheBase cacheWrapperBase = new DataCache<T1, T2>();
            return cacheData.TryRemove(daoKey, out cacheWrapperBase);
        }

        public DataCache<T1, T2> GetDataCacheWrapper<T1, T2>()
        where T1 : DaoBase<T2>, new()
        where T2 : DtoBase<T2>, new()
        {
            Type daoKey = typeof(T1);
            if (!IsRegisted(daoKey))
            {
                return null;
            }
            return (DataCache<T1, T2>)cacheData[daoKey];
        }

        public void ClearAll()
        {
            foreach(CacheBase dataCacheWrapper in Instance.cacheData.Values)
            {
                dataCacheWrapper.Clear();
            }
        }

        public IEnumerator<float> ReadAllRegistoryData()
        {
            var allCount = Instance.cacheData.Count;
            var currentLoadCount = 0;
            if(allCount == 0)
            {
                yield return 1f;
            }

            foreach (CacheBase dataCacheWrapper in Instance.cacheData.Values)
            {
                dataCacheWrapper.Load();
                currentLoadCount++;
                yield return currentLoadCount / allCount;
            }
        }

        public IEnumerator<float> WriteAllRegistoryData()
        {
            var allCount = Instance.cacheData.Count;
            var currentWiteCount = 0;
            if(allCount == 0)
            {
                yield return 1f;
            }
            foreach (CacheBase dataCacheWrapper in Instance.cacheData.Values)
            {
                dataCacheWrapper.Write();
                currentWiteCount++;
                yield return currentWiteCount / allCount;
            }
        }
    }
}
