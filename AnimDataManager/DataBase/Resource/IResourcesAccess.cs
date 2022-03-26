using System;
using System.Collections.Generic;
using System.Text;

namespace AnimDataManager.DataBase.Resource
{
    public interface IResourcesAccess<T>
    {
        void Action(Action<T> action);
        RT Action<RT>(Func<T, RT> action);
    }
}
