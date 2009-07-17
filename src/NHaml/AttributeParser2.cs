using System;
using System.Collections.Generic;
using NHaml.Exceptions;

namespace NHaml
{
    public class AttributeParser2
    {
        private readonly string attributesString;
        private string currentKey;
        private string currentValue;
        private ParsedAttributeType attributeType;
        LinkedList<Token> tokens;
        public List<ParsedAttribute> Attributes { get; private set; }

        public AttributeParser2(string attributesString)
        {
            Attributes = new List<ParsedAttribute>();
            this.attributesString = attributesString;
        }


        public void Parse()
        {
            Attributes.Clear();
            tokens = Token.ReadAllTokens(attributesString);
            Token next;
            while (!(next=tokens.First.Value).IsEnd)
            {
                if (next.IsWhiteSpace)
                {
                    tokens.RemoveFirst();
                    continue;
                }
                if ((next.Character) == '\'' || (next.IsEscaped) || (next.Character) == '"' || (next.Character) == ':'  || (next.Character) == '}' || (next.Character) == '=')
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

        private void EatWhiteSpace()
        {
            while (tokens.First.Value.IsWhiteSpace)
            {
                tokens.RemoveFirst();
            }
        }
        private void ProcessKey()
        {
            while (true)
            {
                var token = tokens.First.Value;
                tokens.RemoveFirst();
                currentKey += token.Character;

                var next = tokens.First.Value;
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

            var next = tokens.First.Value;
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
            tokens.RemoveFirst();
            EatWhiteSpace();
            var next = tokens.First;
            var character = next.Value.Character;

            if ((character == '\\') || (next.Value.IsEnd))
            {
                throw new Exception();
            }
            if ((character == '\'') || (character == '"') )
            {
                ProcessQuotedValue(character);
                return;
            }
            if (character == '#' && next.Next.Value.Character == '{')
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
                var token = tokens.First.Value;
                tokens.RemoveFirst();
                currentValue += token.Character;

                var next = tokens.First.Value;
                if ((next.Character == ' ') || next.IsEnd)
                {
                    return;
                }
            }
        }

        private void ProcessHashedCode()
        {
            attributeType = ParsedAttributeType.Expression;
            tokens.RemoveFirst();
            tokens.RemoveFirst();
            while (true)
            {
                var token = tokens.First.Value;
                tokens.RemoveFirst();
                currentValue += token.Character;

                var next = tokens.First.Value;
                if (next.IsEnd)
                {
                    throw new SyntaxException();
                }
                if (!next.IsEscaped && (next.Character == '}'))
                {
                    tokens.RemoveFirst();
                    return;
                }
            }
        }

        private void ProcessQuotedValue(char character)
        {
            attributeType = ParsedAttributeType.String;
            tokens.RemoveFirst();
            while (true)
            {
                var token = tokens.First.Value;
                tokens.RemoveFirst();
                currentValue += token.Character;

                var next = tokens.First.Value;
                if (next.IsEnd)
                {
                    throw new SyntaxException();
                }
                if (!next.IsEscaped && (next.Character == character))
                {
                    tokens.RemoveFirst();
                    break;
                }
            }
        }


        private void CheckForDuplicates()
        {
            foreach (var attribute in Attributes)
            {
                var parsedAttribute = attribute;
                if (Attributes.Find(x=> parsedAttribute != x &&
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
            Attributes.Add(new ParsedAttribute {Name = name, Schema = schema, Type = attributeType, Value = currentValue});
        }



    }
}