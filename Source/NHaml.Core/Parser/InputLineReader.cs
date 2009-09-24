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

            var line = _reader.ReadLine();

            Next = line != null ? new InputLine(line, LineNumber) : null;
        }
    }
}