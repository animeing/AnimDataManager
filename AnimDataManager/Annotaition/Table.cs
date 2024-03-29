﻿using System;

namespace AnimDataManager.Annotaition
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class Table : Attribute
    {
        public string name {
            get;
            private set;
        }

        public Table(string name)
        {
            this.name = name;
        }
    }
}
