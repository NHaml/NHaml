using System.IO;
using NHaml.Core.Ast;

namespace NHaml.Core.TemplateResolution
{
    /// <summary>
    /// Represents a view template source on the file system.
    /// </summary>
    public class FileViewSource : IViewSource
    {
        private readonly FileInfo _fileInfo;
        private long _lastUpdated;
        private DocumentNode _result;

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
            _result = null;
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

        public DocumentNode ParseResult
        {
            get {
                if ((_result == null) || (IsModified))
                {
                    var parser = new Parser.Parser();
                    _result = parser.ParseFile(_fileInfo);
                }
                return _result;
            }
        }
    }
}