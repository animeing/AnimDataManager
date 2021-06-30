using AnimDataManager.DataBase;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace AnimDataManager.AutoLoader
{
    public sealed class DataCache<T1, T2>
        where T1 : DaoBase<T2>, new()
        where T2 : DtoBase<T2>, new()
    {
        private DataCache() {}
        public static DataCache<T1, T2> Instance { get; } = new DataCache<T1, T2>();
        private ConcurrentDictionary<Type, DataCacheWrapper<T1, T2>> cacheData = new ConcurrentDictionary<Type, DataCacheWrapper<T1, T2>>();

        public bool IsRegisted
        {
            get
            {
                return cacheData.ContainsKey(typeof(T1));
            }
        }

        public bool Regist()
        {
            if(IsRegisted)
            {
                return false;
            }
            return cacheData.TryAdd(typeof(T1), new DataCacheWrapper<T1, T2>());
        }

        public bool Erase()
        {
            if (!IsRegisted)
            {
                return false;
            }
            DataCacheWrapper<T1, T2> cache = new DataCacheWrapper<T1, T2>();
            return cacheData.TryRemove(typeof(T1), out cache);
        }

        public bool Add(T2 data)
        {
            if (!IsRegisted)
            {
                return false;
            }
            return cacheData[typeof(T1)].Add(data);
        }

        public bool Remove(T2 data)
        {
            if (!IsRegisted)
            {
                return false;
            }
            return cacheData[typeof(T1)].Remove(data);
        }

        public void Clear()
        {
            if (!IsRegisted)
            {
                return;
            }
            cacheData[typeof(T1)].Clear();
        }

        public void ClearAll()
        {
            foreach(DataCacheWrapper<T1, T2> dataCacheWrapper in Instance.cacheData.Values)
            {
                dataCacheWrapper.Clear();
            }
        }

        public ICollection<T2> GetCacheData()
        {
            if(!IsRegisted)
            {
                return Enumerable.Empty<T2>().ToList();
            }
            return cacheData[typeof(T1)].Cache.Values;
        }

        public bool GetCacheData(ref T2 data)
        {
            if (!IsRegisted)
            {
                return false;
            }
            DataCacheWrapper<T1, T2> cacheWrapper = cacheData[typeof(T1)];
            string key = cacheWrapper.CreateKey(data);
            if (!cacheWrapper.Cache.ContainsKey(key))
            {
                return false;
            }
            data = cacheWrapper.Cache[key];
            return true;
        }

        public IEnumerator<float> ReadAllRegistoryData()
        {
            var allCount = Instance.cacheData.Count;
            var currentLoadCount = 0;

            foreach (DataCacheWrapper<T1, T2> dataCacheWrapper in Instance.cacheData.Values)
            {
                dataCacheWrapper.Load();
                currentLoadCount++;
                yield return currentLoadCount / allCount;
            }
        }

        public void ReadData()
        {
            if (!IsRegisted)
            {
                return;
            }
            Instance.cacheData[typeof(T1)].Load();
        }

        public IEnumerator<float> WriteAllRegistoryData()
        {
            var allCount = Instance.cacheData.Count;
            var currentWiteCount = 0;
            foreach (DataCacheWrapper<T1, T2> dataCacheWrapper in Instance.cacheData.Values)
            {
                dataCacheWrapper.Write();
                currentWiteCount++;
                yield return currentWiteCount / allCount;
            }
        }

        public void WriteData()
        {
            if (!IsRegisted)
            {
                return;
            }
            Instance.cacheData[typeof(T1)].Write();
        }
    }
}
