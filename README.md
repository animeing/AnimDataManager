# AnimDataManager
これは私がジェネリクスの勉強のために作ったC#でのDTO、DAOの基礎コードです。

## 使い方例
### DAO
```C#
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
    public Save()
    {
        DataCache<ItemDao, ItemDto> manager = DataCache<ItemDao, ItemDto>.Instance;
        //ItemDto、ItemDaoをCacheの管理下に登録
        manager.Regist();
        //cacheに入れるDtoの作成
        ItemDto itemDto = new ItemDto();
        itemDto.Count = 99;
        itemDto.Name = "Item";
        //cacheに追加
        manager.Add(itemDto);
        //cacheを外部に保存
        IEnumrator<float> write = manager.WriteAllRegistoryData();
        while(write.MoveNext()){}
    }
}
```

### Load
```C#
public class Load
{
    public Load()
    {
        DataCache<ItemDao, ItemDto> manager = DataCache<ItemDao, ItemDto>.Instance;
        //ItemDto、ItemDaoをCacheの管理下に登録
        manager.Regist();
        //外部に保存されたデータを読み込み
        IEnumrator<float> load = manager.ReadAllRegistoryData();
        while(load.MoveNext()){}
    }
}
```
