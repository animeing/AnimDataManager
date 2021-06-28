using System;

namespace AnimDataManager.Annotaition
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class Column : Attribute
    {
        public string name {
            get;
            private set;
        }
        public DataType dataType
        {
            get;
            private set;
        }
        public bool isUnique
        {
            get;
            private set;
        }

        public Column(DataType dataType, string name, bool isUnique)
        {
            this.dataType = dataType;
            this.name = name;
            this.isUnique = isUnique;
        }
        public Column(DataType dataType, string name)
        {
            this.dataType = dataType;
            this.name = name;
            isUnique = false;
        }
    }
}
