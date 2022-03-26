using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static AnimDataManager.DataBase.Resource.FileResource;

namespace AnimDataManager.DataBase.Resource
{
    class FileResource : ExclusiveLockResource<CreateStream>
    {

        internal delegate FileStream CreateStream(FileMode fileMode);
        public FileResource(string filePath) : base(delegate (FileMode fileMode) { return new FileStream(filePath, fileMode); }){}

    }
}
