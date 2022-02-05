using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Core.Utilities.FileWatcher.Abstract
{
    public interface IFileWatcher : IDisposable
    {
        public IFileWatcher CreateWatcher(string fileType, string path);
        string GetFilePath();

        public event FileSystemEventHandler InvokeOnChange;
    }
}
