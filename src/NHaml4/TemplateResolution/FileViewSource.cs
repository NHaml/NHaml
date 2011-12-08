using System.IO;

namespace NHaml4.TemplateResolution
{
    /// <summary>
    /// Represents a view template source on the file system.
    /// </summary>
    public class FileViewSource : IViewSource
    {
        private readonly FileInfo _fileInfo;
        private long _lastUpdated;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileViewSource"/> class.
        /// </summary>
        /// <param name="fileInfo">The file info.</param>
        public FileViewSource(FileInfo fileInfo)
        {
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