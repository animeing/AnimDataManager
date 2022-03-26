using System;
using System.Collections.Generic;
using System.Text;

namespace AnimDataManager.DataBase.Resource
{
    public class ExclusiveLockResource<R> : IResourcesAccess<R>
    {
        protected readonly object lockObject = new object();
        protected R resource;

        public ExclusiveLockResource(R resource)
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

        public RT Action<RT>(Func<R, RT> action)
        {
            lock(lockObject)
            {
                return action(resource);
            }
        }

    }
}
