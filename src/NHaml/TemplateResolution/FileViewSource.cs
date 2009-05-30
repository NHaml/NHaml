using System.IO;

namespace NHaml.TemplateResolution
{
    /// <summary>
    /// Represents a view template source on the file system.
    /// </summary>
    public class FileViewSource : IViewSource
    {
        private FileInfo fileInfo;
        private long _lastUpdated;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileViewSource"/> class.
        /// </summary>
        /// <param name="fileInfo">The file info.</param>
        public FileViewSource(FileInfo fileInfo)
        {
            this.fileInfo = fileInfo;
            _lastUpdated = LastModified;
        }

        public StreamReader GetStreamReader()
        {
            _lastUpdated = LastModified;

            return new StreamReader(fileInfo.FullName);
        }

        public string Path
        {
            get { return fileInfo.FullName; }
        }


        private long LastModified
        {
            get { return File.GetLastWriteTime(fileInfo.FullName).Ticks; }
        }

        public bool IsModified
        {
            get
            {
                return _lastUpdated < LastModified;
            }
        }
    }
}