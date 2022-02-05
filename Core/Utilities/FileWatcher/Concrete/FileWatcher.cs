using Core.Utilities.FileWatcher.Abstract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Core.Utilities.FileWatcher.Concrete
{
    public class FileWatcher : IFileWatcher
    {
        public event FileSystemEventHandler InvokeOnChange;
        private readonly FileSystemWatcher FileSystemWatcher;
        public FileWatcher()
        {
            FileSystemWatcher = new FileSystemWatcher();
        }

        public IFileWatcher CreateWatcher(string fileType, string path)
        {
            FileSystemWatcher.Filter = new StringBuilder("*.").Append(fileType).ToString();
            FileSystemWatcher.Path = path;
            FileSystemWatcher.IncludeSubdirectories = false;
            FileSystemWatcher.EnableRaisingEvents = true;
            FileSystemWatcher.Created += Invoke;
            return this;
        }

        private void Invoke(object sender, FileSystemEventArgs e)
        {            
            InvokeOnChange.Invoke(sender, e);
        }
        public string GetFilePath()
        {
            return new StringBuilder(FileSystemWatcher.Path).Append("\\").Append(FileSystemWatcher.Filter).ToString();
        }

        public void Dispose()
        {
            FileSystemWatcher.Dispose();
        }
    }
}
