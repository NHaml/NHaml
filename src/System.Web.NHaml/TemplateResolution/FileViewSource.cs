using System.IO;

namespace System.Web.NHaml.TemplateResolution
{
    public class FileViewSource : ViewSource
    {
        private readonly FileInfo _fileInfo;

        public FileViewSource(FileInfo fileInfo)
        {
            if (fileInfo == null) throw new ArgumentNullException("fileInfo");

            fileInfo.Refresh();
            if (!fileInfo.Exists)
            {
                throw new FileNotFoundException("FileNotFound", fileInfo.FullName);
            }
            _fileInfo = fileInfo;
        }

        public override TextReader GetTextReader()
        {
            return new StreamReader(_fileInfo.FullName);
        }

        public override string FilePath
        {
            get { return _fileInfo.FullName; }
        }

        public override string FileName
        {
            get
            {
                return _fileInfo.Name;
            }
        }

        public override DateTime TimeStamp
        {
            get { return File.GetLastWriteTime(FilePath); }
        }
    }
}