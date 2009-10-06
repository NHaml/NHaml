using System;
using System.Collections.Generic;
using NHaml.Core.Ast;

namespace NHaml.Core.Parser
{
    public class AttributeParser
    {
        private readonly CharacterReader _reader;
        private readonly ParserReader _parser;

        public AttributeParser(CharacterReader reader, ParserReader parser)
        {
            if(reader == null)
                throw new ArgumentNullException("reader");
            if(parser == null)
                throw new ArgumentNullException("parser");

            _reader = reader;
            _parser = parser;
        }

        public IEnumerable<AttributeNode> ParseHtmlStyle()
        {
            _reader.Read(); // eat (

            while(_reader.Current != ')')
            {
                _reader.ReadWhiteSpaces();

                if(!ReadNextLineIfEof())
                {
                    // report error
                    break;
                }

                var name = _reader.ReadName();

                _reader.ReadWhiteSpaces();

                //Todo: report error when there is no =
                _reader.Read(); // =

                _reader.ReadWhiteSpaces();

                var attribute = new AttributeNode(name);

                switch(_reader.Current)
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
            }
        }

        public IEnumerable<AttributeNode> ParseRubyStyle()
        {
            _reader.Read(); // eat {

            while(_reader.Current != '}')
            {
                _reader.ReadWhiteSpaces();

                if(!ReadNextLineIfEof())
                {
                    // report error 
                    break;
                }

                var name = ReadRubyStyleName();

                _reader.ReadWhiteSpaces();

                //Todo: report error when there is no =>
                _reader.Read(2); // =>

                _reader.ReadWhiteSpaces();

                var attribute = new AttributeNode(name);
                
                switch(_reader.Current)
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

                _reader.ReadWhiteSpaces();

                //if(reader.Current!='}'&&reader.Current!=',')
                // report error here

                if(_reader.Current != '}')
                    _reader.Read(); // eat ,
            }        
        }

        private string ReadRubyStyleName()
        {
            string name = null;
            
            switch(_reader.Current)
            {
                case ':':
                {
                    _reader.Read(); // eat :
                    name = _reader.ReadName();
                    break;
                }
                case '\'':
                {
                    _reader.Read(); // eat '
                    
                    name = _reader.ReadWhile(c => c != '\'');
                    
                    _reader.Read(); // eat '
                }
                    break;
                default:
                    _reader.Read(); // eat char
                    break;
            }

            return name;
        }

        private TextNode ReadTickMarkString()
        {
            _reader.Read(); // skip '
            
            var value = _parser.ParseText(_reader.ReadWhile(c => c != '\''));
            
            _reader.Read(); // skip '

            return value;
        }

        private TextNode ReadQuotedString()
        {
            _reader.Read(); // skip "
            
            var value = _parser.ParseText(_reader.ReadWhile(c => c != '"'));

            _reader.Read(); // skip "

            return value;
        }

        private bool ReadNextLineIfEof()
        {
            if(!_reader.IsEndOfStream)
                return true;

            if(!_parser.Read())
                return false;

            _reader.Initialize(_parser.Text);
            
            return _reader.Read();
        }
    }
}