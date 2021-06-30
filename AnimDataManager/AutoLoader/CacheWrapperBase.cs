using AnimDataManager.DataBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AnimDataManager.AutoLoader
{
    public abstract class CacheWrapperBase
    {
        protected abstract string CreateKey <T>(T data) where T : DtoBase<T>, IDtoBase, new();
        protected abstract bool Add<T>(T data) where T : DtoBase<T>, IDtoBase, new();
        protected abstract bool Remove<T>(T data) where T : DtoBase<T>, IDtoBase, new();
        public abstract void Clear();
        public abstract void Load();
        public abstract void Write();

    }
}