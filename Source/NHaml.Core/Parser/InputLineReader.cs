using System;
using System.IO;

namespace NHaml.Core.Parser
{
    public class InputLineReader
    {
        private readonly TextReader _reader;

        public InputLineReader(TextReader reader)
        {
            if(reader == null)
                throw new ArgumentNullException("reader");

            _reader = reader;
            LineNumber = -1;
        }

        public InputLine Prev { get; private set; }

        public InputLine Current { get; private set; }

        public InputLine Next { get; private set; }

        public int LineNumber { get; private set; }

        public bool Read()
        {
            if(Current == null)
                ReadCore();

            LineNumber++;

            ReadCore();

            return Current != null;
        }

        private void ReadCore()
        {
            Prev = Current;
            Current = Next;
            Next = ReadInputLine();
        }

        private InputLine ReadInputLine()
        {
            var line = _reader.ReadLine();

            if(line == null)
                return null;

            var lineWithoutIndent = line.TrimStart();
            var lineWithTrimedEnd = lineWithoutIndent.TrimEnd();

            var startIndex = line.Length - lineWithoutIndent.Length;

            var indent = startIndex/2; //Todo: better indent strategie

            if(lineWithTrimedEnd.EndsWith("|"))
            {
                line = lineWithTrimedEnd.Substring(0, lineWithTrimedEnd.Length - 1);
                return new InputLine(line, LineNumber + 1, indent, startIndex, false);
            }

            return new InputLine(lineWithoutIndent, LineNumber + 1, indent, startIndex, true);
        }
    }
}