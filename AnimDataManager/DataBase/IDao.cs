using System.Collections.Generic;

namespace AnimDataManager.DataBase
{
    public interface IDao<T>
        where T : DtoBase<T>, new()
    {
        List<T> FindAll();
        bool Update(T[] data);
        bool Delete(T[] data);
        bool Insert(T[] data);
    }
}