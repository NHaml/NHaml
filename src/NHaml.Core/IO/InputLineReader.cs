using System;
using System.IO;
using System.Text;

namespace NHaml.Core.IO
{
    public class InputLineReader
    {
        private readonly TextReader _reader;

        /// These variables hold the overall character position of the beginning of the current, next and next-next line.
        private int _oldInputPosition;
        private int _inputPosition;
        private int _inputLineEndPosition;

        public InputLineReader(TextReader reader)
        {
            if(reader == null)
                throw new ArgumentNullException("reader");

            _reader = reader;
            _inputPosition = -1;
            _oldInputPosition = -1;
            _inputLineEndPosition = 0;
            LineNumber = -1;
        }

        public InputLine Prev { get; private set; }

        public InputLine Current { get; private set; }

        public InputLine Next { get; private set; }

        public int LineNumber { get; private set; }

        public int LineBeginPosition { get { return _oldInputPosition; } }

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

        /// <summary>
        /// Reads a line from the input text. Also adds the line delimeter
        /// to the returned text.
        /// Returns an empty string in case of end of file
        /// </summary>
        /// <returns>The next line from the input</returns>
        private string ReadLine()
        {
            _oldInputPosition = _inputPosition;
            _inputPosition = _inputLineEndPosition;
            StringBuilder line = new StringBuilder();
            int r = _reader.Read();
            while (r >= 0)
            {
                char c = Convert.ToChar(r);
                line.Append(c);
                if (c == '\n') break;
                if (c == '\r')
                {
                    int peek = _reader.Peek();
                    if (peek == -1) break;
                    if (Convert.ToChar(peek) != '\n') break;
                }
                r = _reader.Read();
            }
            _inputLineEndPosition = _inputPosition + line.Length;
            return line.ToString();
        }

        private InputLine ReadInputLine()
        {

            /// We use a new ReadLine command to track of the overall character position, because the
            /// built-in removes the end line characters, making it harder to track the actual position
            /// in the stream. However we don't really care about the line delimiter character,
            /// so we throw it away later on.
            var line = ReadLine();

            if(line == "")
                return null;

            line = line.TrimEnd('\r', '\n');

            var lineWithoutIndent = line.TrimStart();
            var lineWithTrimedEnd = lineWithoutIndent.TrimEnd();

            var startIndex = line.Length - lineWithoutIndent.Length;

            var indent = startIndex/2; //Todo: better indent strategie

            if(lineWithTrimedEnd.EndsWith("|"))
            {
                line = lineWithTrimedEnd.Substring(0, lineWithTrimedEnd.Length - 1);
                return new InputLine(line, _inputPosition, LineNumber + 1, indent, startIndex, false);
            }

            return new InputLine(lineWithoutIndent, _inputPosition, LineNumber + 1, indent, startIndex, true);
        }
    }
}