using System;
using System.Collections.Generic;
using System.IO;
using NHaml.Exceptions;

namespace NHaml
{
    public class AttributeParser
    {
        private readonly string attributesString;
        private string currentKey;
        private string currentValue;
        private ParsedAttributeType attributeType;
        TokenReader tokenReader;
        public List<ParsedAttribute> Attributes { get; private set; }

        public AttributeParser(string attributesString)
        {
            Attributes = new List<ParsedAttribute>();
            this.attributesString = attributesString;
        }


        public void Parse()
        {
            Attributes.Clear();
            using (var stringReader = new StringReader(attributesString))
            {
                tokenReader = new TokenReader(stringReader);

                Token next;
                while (!(next = tokenReader.Peek()).IsEnd)
                {
                    if (next.IsWhiteSpace)
                    {
                        tokenReader.Read();
                        continue;
                    }
                    if ((next.Character) == '\'' || (next.IsEscaped) || (next.Character) == '"' || (next.Character) == ':' || (next.Character) == '}' || (next.Character) == '=')
                    {
                        throw new SyntaxException();
                    }
                    ProcessKey();
                    AddCurrent();
                    currentValue = null;
                    currentKey = null;
                }

                CheckForDuplicates();
            }
        }

        private void EatWhiteSpace()
        {
            while (tokenReader.Peek().IsWhiteSpace)
            {
                tokenReader.Read();
            }
        }
        private void ProcessKey()
        {
            while (true)
            {
                var token = tokenReader.Read();
                currentKey += token.Character;

                var next = tokenReader.Peek();
                if (next.IsEscaped)
                {
                    throw new Exception();
                }
                if (next.IsWhiteSpace)
                {
                    ProcessPreEquals();
                    break;
                }
                if (next.Character == '=')
                {
                    ProcessEquals();
                    break;
                }
                if (next.IsEnd)
                {
                    break;
                }
            }
        }

        private void ProcessPreEquals()
        {
            EatWhiteSpace();

            var next = tokenReader.Peek();
            if (next.IsEscaped)
            {
                throw new Exception();
            }
            if ((next.Character == '\'') || (next.Character == '"') || (next.Character == '\\'))
            {
                throw new Exception();
            }
            if (next.Character == '=')
            {
                ProcessEquals();
            }
        }

        private void ProcessEquals()
        {
            tokenReader.Read();
            EatWhiteSpace();
            var next = tokenReader.Peek();
            var character = next.Character;

            if ((character == '\\') || (next.IsEnd))
            {
                throw new SyntaxException();
            }
            if ((character == '\'') || (character == '"'))
            {
                ProcessQuotedValue(character);
                return;
            }
            if (character == '#' && tokenReader.Peek2().Character == '{')
            {
                ProcessHashedCode();
                return;
            }

            ProcessUnQuotedCode();

        }

        private void ProcessUnQuotedCode()
        {
            attributeType = ParsedAttributeType.Reference;
            while (true)
            {
                var token = tokenReader.Read();
                AddCurrentValue(token);

                var next = tokenReader.Peek();
                if ((next.Character == ' ') || next.IsEnd)
                {
                    return;
                }
            }
        }

        private void ProcessHashedCode()
        {
            attributeType = ParsedAttributeType.Expression;
            tokenReader.Read();
            tokenReader.Read();
            while (true)
            {
                var token = tokenReader.Read();
                AddCurrentValue(token);

                var next = tokenReader.Peek();
                if (next.IsEnd)
                {
                    throw new SyntaxException();
                }
                if (!next.IsEscaped && (next.Character == '}'))
                {
                    tokenReader.Read();
                    return;
                }
            }
        }

        private void AddCurrentValue(Token token)
        {
            if (token.IsEscaped)
            {
                currentValue += @"\";
            }
            currentValue += token.Character;
        }

        private void ProcessQuotedValue(char character)
        {
            attributeType = ParsedAttributeType.String;
            currentValue = string.Empty;
            tokenReader.Read();
            while (true)
            {
                var next = tokenReader.Peek();
                if (next.IsEnd)
                {
                    throw new SyntaxException();
                }
                if (!next.IsEscaped && (next.Character == character))
                {
                    tokenReader.Read();
                    break;
                }
                var token = tokenReader.Read();
                AddCurrentValue(token);

            }
        }


        private void CheckForDuplicates()
        {
            foreach (var attribute in Attributes)
            {
                var parsedAttribute = attribute;
                if (Attributes.Find(x => parsedAttribute != x &&
                                          string.Equals(parsedAttribute.Schema, x.Schema, StringComparison.InvariantCultureIgnoreCase) &&
                                          string.Equals(parsedAttribute.Name, x.Name, StringComparison.InvariantCultureIgnoreCase)) != null)
                {

                    throw new SyntaxException(string.Format("The attribute '{0}' is occurs twice.", attribute.Name));
                }
            }
        }


        private void AddCurrent()
        {
            var strings = currentKey.Split(':');
            string schema;
            string name;
            switch (strings.Length)
            {
                case 1:
                    schema = null;
                    name = currentKey;
                    break;
                case 2:
                    schema = strings[0];
                    name = strings[1];
                    if (name == String.Empty)
                    {
                        throw new SyntaxException("Schema with no name.");
                    }
                    break;
                default:
                    throw new SyntaxException("only 1 ':' allowed in key");
            }

            if (currentValue == null)
            {
                currentValue = name;
            }
            Attributes.Add(new ParsedAttribute { Name = name, Schema = schema, Type = attributeType, Value = currentValue });
        }



    }
}