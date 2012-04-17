using System.Collections.Generic;

namespace NHaml4.IO
{
    public class HamlFile
    {
        private readonly IList<HamlLine> _hamlLines = new List<HamlLine>();
        private readonly string _fileName;
        private int _currentLineIndex = 0;

        public HamlFile(string fileName)
        {
            _fileName = fileName;
        }

        public int LineCount
        {
            get { return _hamlLines.Count; }
        }

        public string FileName
        {
            get { return _fileName; }
        }

        public HamlLine CurrentLine
        {
            get {
                return _currentLineIndex < _hamlLines.Count
                    ? _hamlLines[_currentLineIndex]
                    : null;
            }
        }

        public void AddLine(HamlLine line)
        {
            _hamlLines.Add(line);
        }
        
        public void MoveNext()
        {
            _currentLineIndex++;
        }

        public bool EndOfFile
        {
            get
            {
                return _currentLineIndex >= _hamlLines.Count;
            }
        }
    }
}
