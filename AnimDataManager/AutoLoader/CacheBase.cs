using AnimDataManager.DataBase;

namespace AnimDataManager.AutoLoader
{
    abstract class CacheBase
    {
        protected abstract string CreateKey <T>(T data) where T : DtoBase<T>, IDtoBase, new();
        public abstract void Clear();
        public abstract void Load();
        public abstract bool Write();

    }
}