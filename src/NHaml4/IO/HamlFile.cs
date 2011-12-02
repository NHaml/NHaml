using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHaml4.IO;
using System.Collections.ObjectModel;

namespace NHaml.IO
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

        public object Read()
        {
            throw new NotImplementedException();
        }

        public HamlLine CurrentLine
        {
            get {
                return _currentLineIndex < _hamlLines.Count
                    ? _hamlLines[_currentLineIndex]
                    : null;
            }
        }

        internal void AddLine(string currentLine)
        {
            _hamlLines.Add(new HamlLine(currentLine));
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
