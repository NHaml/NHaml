using System.IO;
using System;

namespace NHaml4.TemplateResolution
{
    public class FileViewSource : IViewSource
    {
        private readonly FileInfo _fileInfo;
        private long _lastUpdated;

        public FileViewSource(FileInfo fileInfo)
        {
            if (fileInfo == null) throw new ArgumentNullException("fileInfo");

            fileInfo.Refresh();
            if (!fileInfo.Exists)
            {
                throw new FileNotFoundException("FileNotFound", fileInfo.FullName);
            }
            _fileInfo = fileInfo;
            _lastUpdated = LastModified;
        }

        public StreamReader GetStreamReader()
        {
            _lastUpdated = LastModified;

            return new StreamReader(_fileInfo.FullName);
        }

        public string Path
        {
            get { return _fileInfo.FullName; }
        }

        private long LastModified
        {
            get { return File.GetLastWriteTime(_fileInfo.FullName).Ticks; }
        }

        public bool IsModified
        {
            get { return _lastUpdated < LastModified; }
        }
    }
}