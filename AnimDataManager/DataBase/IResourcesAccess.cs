using System;
using System.Collections.Generic;
using System.Text;

namespace AnimDataManager.DataBase
{
    public interface IResourcesAccess<T>
    {
        void Action(Action<T> action);
    }
}
