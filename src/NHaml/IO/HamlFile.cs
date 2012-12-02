using System.Collections.Generic;

namespace NHaml.IO
{
    public class HamlFile
    {
        private readonly List<HamlLine> _hamlLines = new List<HamlLine>();
        private readonly string _fileName;
        private int _currentLineIndex;

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

        public void AddRange(IEnumerable<HamlLine> lines)
        {
            _hamlLines.AddRange(lines);
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
