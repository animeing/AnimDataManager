using AnimDataManager.Annotaition;
using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace AnimDataManager.DataBase
{
    [Serializable]
    public abstract class DtoBase<T> : ISerializable
        where T : DtoBase<T>, new()
    {
        public string tableName
        {
            get
            {
                Table tableAttribute = (Table)Attribute.GetCustomAttribute(GetType().Assembly, typeof(Table));
                return tableAttribute.name;
            }
        }

        public DtoBase()
        {

        }

        public DtoBase(SerializationInfo info, StreamingContext context)
        {
            foreach (PropertyInfo fieldInfo in typeof(T).GetProperties())
            {
                var column = (Column)fieldInfo.GetCustomAttribute(typeof(Column));
                if (column == null)
                {
                    continue;
                }

                switch (column.dataType)
                {
                    case DataType.Boolean:
                        fieldInfo.SetValue(this, info.GetValue(column.name, typeof(bool)));
                        continue;
                    case DataType.Byte:
                        fieldInfo.SetValue(this, info.GetValue(column.name, typeof(byte)));
                        continue;
                    case DataType.Char:
                        fieldInfo.SetValue(this, info.GetValue(column.name, typeof(char)));
                        continue;
                    case DataType.Decimal:
                        fieldInfo.SetValue(this, info.GetValue(column.name, typeof(decimal)));
                        continue;
                    case DataType.Double:
                        fieldInfo.SetValue(this, info.GetValue(column.name, typeof(double)));
                        continue;
                    case DataType.Float:
                        fieldInfo.SetValue(this, info.GetValue(column.name, typeof(float)));
                        continue;
                    case DataType.Integer:
                        fieldInfo.SetValue(this, info.GetValue(column.name, typeof(int)));
                        continue;
                    case DataType.Long:
                        fieldInfo.SetValue(this, info.GetValue(column.name, typeof(long)));
                        continue;
                    case DataType.Short:
                        fieldInfo.SetValue(this, info.GetValue(column.name, typeof(short)));
                        continue;
                    case DataType.String:
                        fieldInfo.SetValue(this, info.GetValue(column.name, typeof(string)));
                        continue;
                }
            }
        }

        protected T Clone()
        {
            var copy = new T();
            foreach (PropertyInfo fieldInfo in typeof(T).GetProperties())
            {
                var column = (Column)fieldInfo.GetCustomAttribute(typeof(Column));
                if (column == null)
                {
                    continue;
                }
                fieldInfo.SetValue(copy, fieldInfo.GetValue(this));
            }
            return copy;
        }

        internal bool EqualField(T dto, params PropertyInfo[] fieldInfos)
        {
            if (GetType() != dto.GetType())
            {
                return false;
            }
            foreach (var fieldInfo in fieldInfos)
            {
                if (!fieldInfo.GetValue(this).Equals(fieldInfo.GetValue(dto)))
                {
                    return false;
                }
            }
            return true;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            foreach (PropertyInfo fieldInfo in typeof(T).GetProperties())
            {
                var column = (Column)fieldInfo.GetCustomAttribute(typeof(Column));
                if (column == null)
                {
                    continue;
                }

                switch (column.dataType)
                {
                    case DataType.Boolean:
                        info.AddValue(column.name, fieldInfo.GetValue(this), typeof(bool));
                        continue;
                    case DataType.Byte:
                        info.AddValue(column.name, fieldInfo.GetValue(this), typeof(byte));
                        continue;
                    case DataType.Char:
                        info.AddValue(column.name, fieldInfo.GetValue(this), typeof(char));
                        continue;
                    case DataType.Decimal:
                        info.AddValue(column.name, fieldInfo.GetValue(this), typeof(decimal));
                        continue;
                    case DataType.Double:
                        info.AddValue(column.name, fieldInfo.GetValue(this), typeof(double));
                        continue;
                    case DataType.Float:
                        info.AddValue(column.name, fieldInfo.GetValue(this), typeof(float));
                        continue;
                    case DataType.Integer:
                        info.AddValue(column.name, fieldInfo.GetValue(this), typeof(int));
                        continue;
                    case DataType.Long:
                        info.AddValue(column.name, fieldInfo.GetValue(this), typeof(long));
                        continue;
                    case DataType.Short:
                        info.AddValue(column.name, fieldInfo.GetValue(this), typeof(short));
                        continue;
                    case DataType.String:
                        info.AddValue(column.name, fieldInfo.GetValue(this), typeof(string));
                        continue;
                }
            }
        }
        public bool IsMatchKey(T dto)
        {
            foreach (PropertyInfo fieldInfo in typeof(T).GetProperties())
            {
                var column = (Column)fieldInfo.GetCustomAttribute(typeof(Column));
                if (column == null || !column.isUnique)
                {
                    continue;
                }
                if (!fieldInfo.GetValue(this).Equals(fieldInfo.GetValue(dto)))
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsEqualsData(T dto)
        {
            foreach (PropertyInfo fieldInfo in typeof(T).GetProperties())
            {
                var column = (Column)fieldInfo.GetCustomAttribute(typeof(Column));
                if (column == null)
                {
                    continue;
                }
                if (!fieldInfo.GetValue(this).Equals(fieldInfo.GetValue(dto)))
                {
                    return false;
                }
            }
            return true;
        }

    }
}
