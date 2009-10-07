using System;
using System.Text;
using NHaml.Core.Ast;

namespace NHaml.Core.Parser
{
    public class TextParser
    {
        private readonly CharacterReader _reader;
        private StringBuilder _buffer;

        public TextParser(CharacterReader reader)
        {
            if(reader == null)
                throw new ArgumentNullException("reader");

            _reader = reader;
        }

        public TextNode Parse()
        {
            var node = new TextNode();
            _buffer = new StringBuilder();

            while(_reader.Read())
                switch(_reader.Current)
                {
                    case '\\': // is possible escaping
                    {
                        if(_reader.Next == '#' || _reader.Next == '\\')
                            _reader.Read(); // eat \

                        goto default;
                    }
                    case '#':
                    {
                        if(_reader.Next == '{')
                        {
                            ParseInterpolation(node);

                            continue;
                        }

                        goto default;
                    }
                    default:
                    {
                        _buffer.Append(_reader.Current);
                        break;
                    }
                }

            if(_buffer.Length > 0)
                node.Chunks.Add(new TextChunk(_buffer.ToString()));

            return node.Chunks.Count == 0 ? null : node;
        }

        private void ParseInterpolation(TextNode node)
        {
            _reader.Read(); // eat #

            if(_buffer.Length > 0)
                node.Chunks.Add(new TextChunk(ReturnAndClearBuffer()));

            while(_reader.Read() && _reader.Current != '}')
            {
                if(_reader.Current == '\\')
                    if(_reader.Next == '}' || _reader.Next == '\\')
                        _reader.Read(); // escaping - eat \

                _buffer.Append(_reader.Current);
            }

            if(_buffer.Length > 0)
                node.Chunks.Add(new CodeChunk(ReturnAndClearBuffer()));
        }

        private string ReturnAndClearBuffer()
        {
            var buffer = _buffer.ToString();

            _buffer = new StringBuilder();

            return buffer;
        }
    }
}