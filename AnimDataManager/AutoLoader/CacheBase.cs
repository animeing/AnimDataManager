using AnimDataManager.DataBase;
using AnimDataManager.DataBase.Dto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AnimDataManager.AutoLoader
{
    public abstract class CacheBase
    {
        protected abstract string CreateKey <T>(T data) where T : DtoBase<T>, IDtoBase, new();
        public abstract void Clear();
        public abstract void Load();
        public abstract bool Write();

    }
}