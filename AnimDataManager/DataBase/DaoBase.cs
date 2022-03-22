using AnimDataManager.Annotaition;
using System;
using System.Collections.Generic;

namespace AnimDataManager.DataBase
{
    public abstract class DaoBase<T> : IDao<T>
        where T : DtoBase<T>, new()
    {
        public string tableName
        {
            get
            {
                Table tableAttribute = (Table)Attribute.GetCustomAttribute(typeof(T).Assembly, typeof(Table));
                if(tableAttribute == null)
                {
                    return "";
                }
                return tableAttribute.name;
            }
        }

        private static readonly object lockObject = new object();

        protected void ExclusiveLockAction(Action action)
        {
            lock(lockObject)
            {
                action();
            }
        }

        public abstract List<T> FindAll();
        public abstract bool Update(T[] data);
        public abstract bool Delete(T[] data);
        public abstract bool Insert(T[] data);
    }
}
