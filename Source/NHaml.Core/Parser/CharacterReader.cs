using System;
using System.IO;
using System.Text;

namespace NHaml.Core.Parser
{
    public class CharacterReader
    {
        private readonly StringReader _reader;

        public CharacterReader(string text)
        {
            if(text == null)
                throw new ArgumentNullException("text");

            _reader = new StringReader(text);
        }

        public char Current { get; private set; }

        public char Next
        {
            get { return (char)_reader.Peek(); }
        }

        public bool IsEndOfStream
        {
            get { return _reader.Peek() == -1; }
        }

        public char Prev { get; private set; }

        public bool Read()
        {
            Prev = Current;

            var value = _reader.Read();

            Current = (char)value;

            return value != -1;
        }

        public bool Read(int count)
        {
            if(count < 0)
                throw new ArgumentOutOfRangeException("count");

            for(var index = 0; index < count; index++)
                if(!Read())
                    return false;

            return Read();
        }

        public string ReadToEnd()
        {
            return Current + _reader.ReadToEnd();
        }

        public string ReadWhile(Predicate<char> predicate)
        {
            if(predicate == null)
                throw new ArgumentNullException("predicate");

            var buffer = new StringBuilder();

            do
            {
                if(!predicate(Current))
                    break;

                buffer.Append(Current);
            }
            while(Read());

            return buffer.ToString();
        }

        public void ReadWhiteSpaces()
        {
            ReadWhile(c => char.IsWhiteSpace(c));
        }

        public string ReadName()
        {
            return ReadWhile(IsNameChar);
        }

        public static bool IsNameChar(char ch)
        {
            return char.IsNumber(ch) ||
                   char.IsLetter(ch) ||
                   ch == '_' ||
                   ch == '-' ||
                   ch == ':';
        }
    }
}