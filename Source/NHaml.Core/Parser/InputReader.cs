using System;
using System.IO;
using NHaml.Core.Ast;

namespace NHaml.Core.Parser
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
            get { return new SourceInfo(_inputLineReader.LineNumber, Index); }
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

        public bool ReadNextLineAndReadIfEndOfStream()
        {
            if(IsEndOfStream)
                return ReadNextLine() && Read();

            return !IsEndOfStream;
        }
    }
}