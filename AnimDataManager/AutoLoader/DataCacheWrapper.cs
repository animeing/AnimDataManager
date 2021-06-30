using AnimDataManager.Annotaition;
using AnimDataManager.DataBase;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace AnimDataManager.AutoLoader
{
    internal class DataCacheWrapper<T1, T2>
        where T2 : DtoBase<T2>, new()
        where T1 : DaoBase<T2>, IDao<T2>, new()
    {

        public ConcurrentDictionary<string, T2> Cache
        {
            get;
            private set;
        } = new ConcurrentDictionary<string, T2>();
        private T1 dao = new T1();
        private List<PropertyInfo> uniques = new List<PropertyInfo>();
        private List<PropertyInfo> others = new List<PropertyInfo>();

        public DataCacheWrapper()
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

        private string CreateKey(T2 data)
        {
            var keys = "";
            foreach (PropertyInfo unique in uniques)
            {
                keys += "|"+(unique.GetValue(data).ToString().Replace("|", "\\|"));
            }
            return keys;
        }

        public bool Add(T2 data)
        {
            string keys = CreateKey(data);
            if (Cache.ContainsKey(keys))
            {
                return Cache.TryUpdate(keys, data, Cache[keys]);
            }
            else
            {
                return Cache.TryAdd(keys, data);
            }
        }

        public bool Remove(T2 data)
        {
            return Cache.TryRemove(CreateKey(data), out data);
        }

        public void Clear()
        {
            Cache.Clear();
        }

        internal void Load()
        {
            ConcurrentDictionary<string, T2> newDto = ToDictionaryDtos(dao.FindAll());
            Cache = newDto;
        }

        internal void Write()
        {
            if (uniques.Count == 0)
            {
                List<T2> currentStore = dao.FindAll();
                List<T2> insert = new List<T2>(Cache.Values);
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
                dao.Delete(remove.ToArray());
                dao.Insert(insert.ToArray());
                return;
            } else {

                ConcurrentDictionary<string, T2> currentStore = ToDictionaryDtos(dao.FindAll());
                var remove = new List<T2>(currentStore.Values);
                var update = new List<T2>();
                var inserts = new List<T2>();
                foreach (string keys in Cache.Keys)
                {
                    if (!currentStore.ContainsKey(keys))
                    {
                        inserts.Add(Cache[keys]);
                        continue;
                    }
                    if (!Cache[keys].EqualField(currentStore[keys], others.ToArray()))
                    {
                        update.Add(Cache[keys]);
                        remove.Remove(currentStore[keys]);
                        continue;
                    }
                }
                dao.Delete(remove.ToArray());
                dao.Update(update.ToArray());
                dao.Insert(inserts.ToArray());
            }
        }
    }
}
