using System;
using System.IO;
using NHaml.Core.IO;
using System.Text;

namespace NHaml.Core.IO
{
    public class InputReader : CharacterReader
    {
        private readonly InputLineReader _inputLineReader;
        
        public InputReader(TextReader reader)
        {
            if(reader == null)
                throw new ArgumentNullException("reader");

            _inputLineReader = new InputLineReader(reader);
        }

        public InputLine PrevLine
        {
            get { return _inputLineReader.Prev; }
        }

        public InputLine CurrentLine
        {
            get { return _inputLineReader.Current; }
        }

        public InputLine NextLine
        {
            get { return _inputLineReader.Next; }
        }

        public SourceInfo SourceInfo
        {
            get { return new SourceInfo(_inputLineReader.LineNumber, Index, _inputLineReader.LineBeginPosition + Index); }
        }

        public int LineNumber
        {
            get { return _inputLineReader.LineNumber; }
        }

        public bool ReadNextLine()
        {
            if(!_inputLineReader.Read())
                return false;

            Initialze(CurrentLine.Text, CurrentLine.StartIndex);

            return true;
        }

        public string ReadToEndMultiLine()
        {
            if (CurrentLine.IsMultiLine)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(ReadToEnd());
                while (NextLine != null && NextLine.IsMultiLine)
                {
                    sb.Append(" ");
                    ReadNextLine();
                    sb.Append(ReadToEnd());
                }
                return sb.ToString();
            }
            else
            {
                return ReadToEnd();
            }
        }

        public bool ReadNextLineAndReadIfEndOfStream()
        {
            if(IsEndOfStream)
                return ReadNextLine() && Read();

            return !IsEndOfStream;
        }
    }
}