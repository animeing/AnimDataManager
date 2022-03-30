using AnimDataManager.DataBase.Dto;
using AnimDataManager.DataBase.Resource;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace AnimDataManager.DataBase.Dao
{
    public class FileStreamSaveDaoBase<T> : DaoBase<T>
        where T : DtoBase<T>, new()
    {

        private FileResource streamRerouce;

        public FileStreamSaveDaoBase(string saveFilePath)
        {
            streamRerouce = new FileResource(saveFilePath);
        }

        public override bool Delete(T[] data)
        {
            List<T> currnetSaveData = FindAll();
            foreach (T deleteData in data)
            {
                T remove = currnetSaveData.Find(searchData => searchData.IsMatchKey(deleteData));
                if(remove != null)
                {
                    currnetSaveData.Remove(remove);
                }
            }
            streamRerouce.Action(createStream => 
            {
                using (Stream stream = createStream(FileMode.OpenOrCreate))
                {
                    if (stream.Length > 0)
                    {
                        var formatter = new BinaryFormatter();
                        formatter.Serialize(stream, currnetSaveData);
                    }
                }
            });
            return true;
        }

        public override List<T> FindAll()
        {
            return streamRerouce.Action(createStream =>
            {
                using (Stream stream = createStream(FileMode.OpenOrCreate))
                {
                    if (stream.Length > 0)
                    {
                        var formatter = new BinaryFormatter();
                        return (List<T>)formatter.Deserialize(stream);
                    }
                    else
                    {
                        return new List<T>();
                    }
                }
            });
        }

        public override bool Insert(T[] data)
        {
            List<T> currentSaveData = FindAll();
            foreach(T insertData in data)
            {
                currentSaveData.Add(insertData);
            }
            return streamRerouce.Action(createStream =>
            {
                using (Stream stream = createStream(FileMode.OpenOrCreate))
                {
                    var formatter = new BinaryFormatter();
                    formatter.Serialize(stream, currentSaveData);
                }
                return true;
            });
        }

        public override bool Update(T[] data)
        {
            List<T> currentSaveData = FindAll();
            foreach (T update in data)
            {
                T find = currentSaveData.Find(searchData => searchData.IsMatchKey(update));
                currentSaveData.Remove(find);
                currentSaveData.Add(update);
            }
            return streamRerouce.Action(createStream =>
            {
                using (Stream stream = createStream(FileMode.OpenOrCreate))
                {
                    var formatter = new BinaryFormatter();
                    formatter.Serialize(stream, currentSaveData);
                }
                return true;
            });
        }
    }
}