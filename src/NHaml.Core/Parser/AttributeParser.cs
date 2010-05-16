using System;
using System.Collections.Generic;
using NHaml.Core.Ast;
using NHaml.Core.IO;

namespace NHaml.Core.Parser
{
    public class AttributeParser
    {
        private readonly InputReader _reader;
        private readonly ParserReader _parser;

        public AttributeParser(ParserReader parser)
        {
            if(parser == null)
                throw new ArgumentNullException("parser");
            _reader = parser.Input;
            _parser = parser;
        }

        public IEnumerable<AttributeNode> ParseHtmlStyle()
        {
            _reader.Skip("(");

            while(_reader.CurrentChar != ')')
            {
                _reader.SkipWhiteSpaces();

                if(!_reader.ReadNextLineAndReadIfEndOfStream())
                {
                    //Todo: report error
                    break;
                }

                var name = _reader.ReadName();

                _reader.SkipWhiteSpaces();

                _reader.Skip("=");

                _reader.SkipWhiteSpaces();

                var attribute = new AttributeNode(name);

                switch(_reader.CurrentChar)
                {
                    case '\'':
                    {
                        attribute.Value = ReadTickMarkString();
                        break;
                    }
                    case '"':
                    {
                        attribute.Value = ReadQuotedString();
                        break;
                    }
                    default:
                    {
                        attribute.Value = new CodeNode(_reader.ReadName());
                        break;
                    }
                }

                yield return attribute;

                _reader.SkipWhiteSpaces();
            }

            _reader.Skip(")");
        }

        public IEnumerable<AttributeNode> ParseRubyStyle()
        {
            _reader.Skip("{");

            while(_reader.CurrentChar != '}')
            {
                _reader.SkipWhiteSpaces();

                if(!_reader.ReadNextLineAndReadIfEndOfStream())
                {
                    //Todo: report error
                    break;
                }

                var name = ReadRubyStyleName();

                _reader.SkipWhiteSpaces();

                _reader.Skip("=>");

                _reader.SkipWhiteSpaces();

                var attribute = new AttributeNode(name);

                switch(_reader.CurrentChar)
                {
                    case '\'':
                    {
                        attribute.Value = ReadTickMarkString();
                        break;
                    }
                    case '"':
                    {
                        attribute.Value = ReadQuotedString();
                        break;
                    }
                    default:
                    {
                        attribute.Value = new CodeNode(_reader.ReadName());
                        break;
                    }
                }

                yield return attribute;

                _reader.SkipWhiteSpaces();

                if(_reader.CurrentChar == ',')
                    _reader.Skip(",");
            }

            _reader.Skip("}");
        }

        private string ReadRubyStyleName()
        {
            string name = null;

            switch(_reader.CurrentChar)
            {
                case ':':
                {
                    _reader.Skip(":");
                    name = _reader.ReadName();
                    break;
                }
                case '\'':
                {
                    _reader.Skip("'");

                    name = _reader.ReadWhile(c => c != '\'');

                    _reader.Skip("'");
                }
                    break;
                default:
                    // Todo: something :-)
                    _reader.Read(); // eat char
                    break;
            }

            return name;
        }

        private TextNode ReadTickMarkString()
        {
            _reader.Skip("'");

            var index = _parser.Index;
            var value = _parser.ParseText(_reader.ReadWhile(c => c != '\''), index);

            _reader.Skip("'");

            return value;
        }

        private TextNode ReadQuotedString()
        {
            _reader.Skip("\"");

            var index = _parser.Index;
            var value = _parser.ParseText(_reader.ReadWhile(c => c != '"'), index);

            _reader.Skip("\""); 

            return value;
        }
    }
}