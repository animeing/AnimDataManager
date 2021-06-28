
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Assets.Scripts.AnimDataController.DataBase
{
    internal class UniquesKey
    {
        object[] keys = new object[0];
        internal UniquesKey(object[] keys)
        {
            this.keys = keys;
        }
    }
}