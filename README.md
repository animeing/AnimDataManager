# AnimDataManager
これは私がジェネリクスの勉強のために作ったC#でのDTO、DAOの基礎コードです。

## 使い方例
### DAO
```C#
  //Streamを用いない場合はDaoBaseを拡張することで実装が可能です。
  public class ItemDao : StreamSaveDaoBase<ItemDto>
  {
#if UNITY_STANDALONE
    private static readonly string folder = UnityEngine.Application.dataPath + "/";
#elif UNITY_ANDROID
    private static readonly string folder = Application.persistentDataPath;
#endif
      public ItemDao() : base(delegate(FileMode mode){return new FileStream(folder+"save.log", mode)})
      {}
  }
```
  
 #### DTO
```C#
    [Table("Item")]
    [Serializable]
    public class ItemDto : DtoBase<ItemDto>
    {
      public ItemDto(SerializationInfo info, StreamingContext context) : base(info, context){}
      public ItemDto() { }
    
      
    [Column(DataType.Integer, "count")]
    public int Count {
        get {
            return count;
        }
        set {
            count = value;
        }
    }

    private int count;

    [Column(DataType.String, "Name", true)]
    public string Name
    {
        get
        {
            return name;
        }
        set
        {
            name = value;
        }
    }
    private string name;
    }
```
### Save
```C#
public class Save
{
    public void Save()
    {
        DataCacheStore manager = DataCacheStore.Instance;
        //CacheStoreの管理下に定義があるか確認
        if(!manager.IsRegisted<ItemDao, ItemDto>())
        {
            //CacheStoreの管理下に定義を登録
            manager.Regist<ItemDao, ItemDto>();
        }
        //cacheに入れるDtoの作成
        ItemDto itemDto = new ItemDto();
        itemDto.Count = 99;
        itemDto.Name = "Item";
        //Itemのcache取得
        var dataCache = manager.GetDataCache<ItemDao, ItemDto>();
        //Itemのcacheに追加
        dataCache.Add(itemDto);
        //Itemの情報をDaoを使って保存
        dataCache.Write();
    }
    
    //登録したデータをすべてセーブする場合(すべてのDAO, DTOがCacheに定義済みの場合に使うことを想定しています。)
    public IEnumerator<float> SaveAll()
    {
        DataCacheStore manager = DataCacheStore.Instance;
        return manager.WriteAllRegistoryData();
    }
}
```

### Load
```C#
public class Load
{
    public void Load()
    {
        DataCacheStore manager = DataCacheStore.Instance;
        //CacheStoreの管理下に定義があるか確認
        if(!manager.IsRegisted<ItemDao, ItemDto>())
        {
            //CacheStoreの管理下に定義を登録
            manager.Regist<ItemDao, ItemDto>();
        }
        //Itemのcache取得
        var dataCache = manager.GetDataCache<ItemDao, ItemDto>();
        //Itemの情報をDaoを通してcacheに取り込みます。
        dataCache.Load();
    }
    //以前に登録したデータをすべてロードする場合(すべてのDAO, DTOがCacheに定義済みの場合に使うことを想定しています。)
    public IEnumerator<float> LoadAll()
    {
        DataCacheStore manager = DataCacheStore.Instance;
        return manager.ReadAllRegistoryData();
    }
}
```
