using System;
using System.IO;
using System.Text;
using NHaml.Core.Exceptions;

namespace NHaml.Core.IO
{
    public class CharacterReader
    {
        private char? _currentChar;
        private char? _nextChar;
        private char? _prevChar;
        private StringReader _reader;

        protected CharacterReader()
        {
        }

        public CharacterReader(string text, int index)
        {
            Initialze(text, index);
        }

        public int Index { get; private set; }

        public char? CurrentChar
        {
            get { return _currentChar; }
        }

        public char? NextChar
        {
            get { return _nextChar; }
        }

        public char? PrevChar
        {
            get { return _prevChar; }
        }

        public bool IsEndOfStream
        {
            get { return !_currentChar.HasValue; }
        }

        protected void Initialze(string text, int index)
        {
            if(text == null)
                throw new ArgumentNullException("text");

            _reader = new StringReader(text);
            _prevChar = null;
            _currentChar = null;
            _nextChar = null;
            Index = index - 1;
        }

        public bool Read()
        {
            if(!_currentChar.HasValue)
                ReadCore();

            ReadCore();

            if (_currentChar.HasValue || _prevChar.HasValue)
            {
                Index++;
            }

            return _currentChar.HasValue;
        }

        protected void ReadCore()
        {
            if(_reader==null)
                return;

            var charNumber = _reader.Read();

            _prevChar = _currentChar;
            _currentChar = _nextChar;

            if(charNumber == -1)
                _nextChar = null;
            else
                _nextChar = (char)charNumber;
        }

        public bool Skip(string skiplist)
        {
            var count = skiplist.Length;

            if (!_currentChar.HasValue)
                if (!Read())
                    throw new ParserException(this,String.Format("Parser error: '{0}' expected, but end of line found",skiplist[0]));

            for (var index = 0; index < count; index++)
            {
                if (_currentChar.Value != skiplist[index])
                    throw new ParserException(this, String.Format("Parser error: '{0}' expected, but '{1}' found", skiplist[index], _currentChar.Value));
                if (!Read() && (index != count-1))
                    throw new ParserException(this, String.Format("Parser error: '{0}' expected, but end of line found", skiplist[0]));
            }

            return true;
        }

        public string ReadToEnd()
        {
            var buffer = new StringBuilder();

            if(_currentChar.HasValue)
                buffer.Append(CurrentChar);

            while(Read())
                buffer.Append(CurrentChar);

            return buffer.ToString();
        }

        public string ReadWhile(Predicate<char> predicate)
        {
            if(predicate == null)
                throw new ArgumentNullException("predicate");

            var buffer = new StringBuilder();

            if(!_currentChar.HasValue && !Read())
                return null;

            do
            {
                if(!predicate(CurrentChar.Value))
                    break;

                buffer.Append(CurrentChar);
            }
            while(Read());

            return buffer.ToString();
        }

        public void SkipWhiteSpaces()
        {
            ReadWhile(c => char.IsWhiteSpace(c));
        }

        public string ReadName()
        {
            return ReadWhile(IsNameChar);
        }

        public string ReadNameEscaped()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(ReadWhile(IsNameChar));
            while (CurrentChar == '\\')
            {
                Skip("\\");
                if (CurrentChar != null)
                {
                    sb.Append(CurrentChar);
                    Read();
                }
                sb.Append(ReadWhile(IsNameChar));
            }
            return sb.ToString();
        }

        private static bool IsNameChar(char ch)
        {
            return char.IsNumber(ch) ||
                   char.IsLetter(ch) ||
                   ch == '_' ||
                   ch == '-' ||
                   ch == ':';
        }
    }
}