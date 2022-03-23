using System;
using System.Collections.Generic;
using System.Text;

namespace AnimDataManager.DataBase
{
    public abstract class DaoExclusiveLockBase<R> : IResourcesAccess<R>
    {
        private readonly object lockObject = new object();
        private R resource;

        public DaoExclusiveLockBase(R resource)
        {
            this.resource = resource;
        }

        public void Action(Action<R> action)
        {
            lock (lockObject)
            {
                action(resource);
            }
        }

    }
}
