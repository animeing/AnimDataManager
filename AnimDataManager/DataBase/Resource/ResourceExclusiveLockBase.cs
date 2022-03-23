using System;
using System.Collections.Generic;
using System.Text;

namespace AnimDataManager.DataBase.Resource
{
    public abstract class ResourceExclusiveLockBase<R> : IResourcesAccess<R>
    {
        private readonly object lockObject = new object();
        private R resource;

        public ResourceExclusiveLockBase(R resource)
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
