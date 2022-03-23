using AnimDataManager.Annotaition;
using AnimDataManager.DataBase.Dao;
using AnimDataManager.DataBase.Dto;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace AnimDataManager.AutoLoader
{
    public sealed class DataCache<T1, T2> : CacheBase
        where T2 : DtoBase<T2>, IDtoBase, new()
        where T1 : DaoBase<T2>, IDao<T2>, new()
    {

        internal ConcurrentDictionary<string, T2> cache  = new ConcurrentDictionary<string, T2>();
        private T1 dao = new T1();
        private readonly static List<PropertyInfo> uniques = new List<PropertyInfo>();
        private readonly static List<PropertyInfo> others = new List<PropertyInfo>();

        public DataCache()
        {
            foreach (PropertyInfo fieldInfo in typeof(T2).GetProperties())
            {
                var column = (Column)fieldInfo.GetCustomAttribute(typeof(Column));
                if (column == null)
                {
                    continue;
                }
                if (!column.isUnique)
                {
                    others.Add(fieldInfo);
                }
                else
                {
                    uniques.Add(fieldInfo);
                }
            }
        }

        private ConcurrentDictionary<string, T2> ToDictionaryDtos(List<T2> dtos)
        {
            var dictionary = new ConcurrentDictionary<string, T2>();
            foreach (T2 dto in dtos)
            {
                var keys = CreateKey(dto);
                dictionary.TryAdd(keys, dto);
            }
            return dictionary;
        }

        public string CreateKey(T2 data)
        {
            return CreateKey<T2>(data);
        }


        public ICollection<T2> GetCache()
        {
            return cache.Values;
        }

        protected override string CreateKey<T>(T data)
        {
            var keys = "";
            foreach (PropertyInfo unique in uniques)
            {
                keys += "|" + (unique.GetValue(data).ToString().Replace("|", "\\|"));
            }
            return keys;
        }

        /// <summary>
        /// Uniqueの値からcacheのデータを取得します。
        /// </summary>
        /// <returns>cacheに値が存在した場合true</returns>
        public bool Find(ref T2 param)
        {
            string key = CreateKey(param.Clone());
            if (!cache.ContainsKey(key))
            {
                return false;
            }
            param = cache[key];
            return true;
        }

        public bool Add(T2 data)
        {
            string keys = CreateKey(data);
            if (cache.ContainsKey(keys))
            {
                cache[keys] = data;
                return true;
            }
            else
            {
                return cache.TryAdd(keys, data);
            }
        }

        public bool Remove(T2 data)
        {
            return cache.TryRemove(CreateKey(data), out data);
        }

        public override void Clear()
        {
            cache.Clear();
        }

        public override void Load()
        {
            ConcurrentDictionary<string, T2> newDto = ToDictionaryDtos(dao.FindAll());
            cache = newDto;
        }

        private bool DaoAction(Func<T2[], bool> action, T2[] data)
        {
            if(data.Length == 0)
            {
                return true;
            }
            return action(data);
        }

        public override bool Write()
        {
            if (uniques.Count == 0)
            {
                List<T2> currentStore = dao.FindAll();
                List<T2> insert = new List<T2>(cache.Values);
                List<T2> remove = new List<T2>(currentStore);
                foreach(T2 current in currentStore)
                {
                    T2 notChanged = insert.Find(data => data.EqualField(current, others.ToArray()));
                    if(notChanged != null)
                    {
                        insert.Remove(notChanged);
                        remove.Remove(current);
                    }
                }
                bool result = true;
                result &= DaoAction(dao.Delete, remove.ToArray());
                result &= DaoAction(dao.Insert, insert.ToArray());
                return result;
            }
            else
            {

                ConcurrentDictionary<string, T2> currentStore = ToDictionaryDtos(dao.FindAll());
                var remove = new List<T2>(currentStore.Values);
                var update = new List<T2>();
                var inserts = new List<T2>();
                foreach (string keys in cache.Keys)
                {
                    if (!currentStore.ContainsKey(keys))
                    {
                        inserts.Add(cache[keys]);
                        continue;
                    }
                    if (!cache[keys].EqualField(currentStore[keys], others.ToArray()))
                    {
                        update.Add(cache[keys]);
                        remove.Remove(currentStore[keys]);
                        continue;
                    }
                    else
                    {
                        remove.Remove(currentStore[keys]);
                    }
                }
                bool result = true;
                result &= DaoAction(dao.Delete, remove.ToArray());
                result &= DaoAction(dao.Update, update.ToArray());
                result &= DaoAction(dao.Insert, inserts.ToArray());
                return result;
            }
        }

    }
}
