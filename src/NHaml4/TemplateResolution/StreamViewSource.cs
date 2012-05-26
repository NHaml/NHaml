using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHaml4.TemplateResolution;
using System.IO;

namespace NHaml4.TemplateResolution
{
    public class StreamViewSource : ViewSource
    {
        private System.IO.TextReader _textReader;
        private string _path;
        private DateTime _timeStamp = DateTime.Now;

        public StreamViewSource(string content, string filePath)
            : this(new MemoryStream(new System.Text.UTF8Encoding().GetBytes(content)), filePath)
        { }
        
        public StreamViewSource(Stream content, string filePath)
            : this(new StreamReader(content), filePath)
        { }

        public StreamViewSource(TextReader reader, string filePath)
        {
            this._textReader = reader;
            this._path = filePath;
        }

        public override System.IO.TextReader GetTextReader()
        {
            return _textReader;
        }

        public override string FilePath
        {
            get { return _path; }
        }

        public override DateTime TimeStamp
        {
            get { return _timeStamp; }
        }

        public override string FileName
        {
            get { return Path.GetFileName(_path); }
        }
    }
}
