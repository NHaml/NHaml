using System.IO;
using System;
using System.Text;

namespace NHaml4.TemplateResolution
{
    public class FileViewSource : IViewSource
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

        public StreamReader GetStreamReader()
        {
            return new StreamReader(_fileInfo.FullName);
        }

        public string FilePath
        {
            get { return _fileInfo.FullName; }
        }

        public string FileName
        {
            get
            {
                return _fileInfo.Name;
            }
        }

        public DateTime TimeStamp
        {
            get { return File.GetLastWriteTime(FilePath); }
        }

        public string GetClassName()
        {
            string templatePath = this.FilePath;
            var stringBuilder = new StringBuilder();
            foreach (char ch in templatePath)
            {
                stringBuilder.Append(Char.IsLetter(ch) ? ch : '_');
            }

            return stringBuilder.ToString();
        }
    }
}