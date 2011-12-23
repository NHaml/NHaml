using System.Collections.Generic;

namespace NHaml4.IO
{
    public class HamlFile
    {
        private IList<HamlLine> _hamlLines = new List<HamlLine>();
        private int _currentLineIndex = 0;

        public int LineCount
        {
            get
            {
                return _hamlLines.Count;
            }
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
