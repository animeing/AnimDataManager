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

        public bool Regist<T1, T2>()
        where T1 : DaoBase<T2>, new()
        where T2 : DtoBase<T2>, new()
        {
            return cacheData.TryAdd(typeof(T1), new DataCache<T1, T2>());
        }

        public bool Erase<T1, T2>()
        where T1 : DaoBase<T2>, new()
        where T2 : DtoBase<T2>, new()
        {
            CacheBase cacheWrapperBase = new DataCache<T1, T2>();
            return cacheData.TryRemove(typeof(T1), out cacheWrapperBase);
        }

        public DataCache<T1, T2> GetDataCacheWrapper<T1, T2>()
        where T1 : DaoBase<T2>, new()
        where T2 : DtoBase<T2>, new()
        {
            return (DataCache<T1, T2>)cacheData[typeof(T1)];
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
            foreach (CacheBase dataCacheWrapper in Instance.cacheData.Values)
            {
                dataCacheWrapper.Write();
                currentWiteCount++;
                yield return currentWiteCount / allCount;
            }
        }
    }
}
