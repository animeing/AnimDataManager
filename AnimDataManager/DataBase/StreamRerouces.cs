using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AnimDataManager.DataBase
{
    class StreamRerouces : IResourcesAccess<Stream>
    {
        public void Action(Action<Stream> action)
        {
        }
    }
}
