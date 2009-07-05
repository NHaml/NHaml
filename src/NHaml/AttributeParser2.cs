using System;
using System.Collections.Generic;
using NHaml.Exceptions;

namespace NHaml
{
    public class AttributeParser2
    {
        enum Mode
        {
            Key,
            PreEquals,
            PostEquals,
            Value
        }
        private readonly string attributesString;

        public AttributeParser2(string attributesString)
        {
            Attributes = new List<ParsedAttribute>();
            this.attributesString = attributesString;
        }

        private Mode? currentMode;
        private string currentKey;
        private string currentValue;
        private bool escaped;
        char currentChar;
        Func<bool> endSignifier;
        private ParsedAttributeType attributeType;
        private int index;


        public List<ParsedAttribute> Attributes { get; private set; }

        //modes 
        //none
        //value startvaluematch
        //key 
        //equals
        public void Parse()
        {
            Attributes.Clear();


            for (index = 0; index < attributesString.Length; index++)
            {
                

                currentChar = attributesString[index];

                if (ProcessValue())
                {
                    continue;
                }
                switch (currentMode)
                {
                    case (null):
                        {
                            ProcessNull();
                            break;
                        }
                    case (Mode.Key):
                        {
                            ProcessKey();
                            break;
                        }
                    case (Mode.PreEquals):
                        {
                            ProcessPreEquals();
                            break;
                        }
                    case (Mode.PostEquals):
                        {
                            ProcessPostEquals();
                            break;
                        }
                    case (Mode.Value):
                        {
                            break;
                        }
                    default:
                        {
                            throw new NotImplementedException();
                        }

                }
            }
            ProcessValue();
            //if (isLastChar && !isEndOfValue)
            //{
            //    throw new SyntaxException(string.Format("Exprected '{0}' but found '{1}'.", endSignifier, currentChar));
            //}
            if ((currentMode == Mode.Key) || (currentMode == Mode.PreEquals))
            {
                attributeType = ParsedAttributeType.String;
                AddCurrent();
                return;
            }
            if (currentMode != null)
            {
                throw new SyntaxException();
            }

            foreach (var attribute in Attributes)
            {
                var parsedAttribute = attribute;
                if (Attributes.Find((x)=> 
                    string.Equals(parsedAttribute.Schema, x.Schema, StringComparison.InvariantCultureIgnoreCase) &&
                    string.Equals(parsedAttribute.Name, x.Name, StringComparison.InvariantCultureIgnoreCase)) != null)
                {
                    
                    throw new SyntaxException(string.Format("The attribute '{0}' is occurs twice.", attribute.Name));
                }
            }
        }

   

        private bool ProcessValue()
        {
            if (currentMode != Mode.Value)
            {
                return false;
            }
            
            var isEndOfValue = endSignifier();
         
            if (!escaped && currentChar == '\\')
            {
                escaped = true;
                return false;
            }
            if (!escaped && isEndOfValue)
            {
                AddCurrent();
                currentMode = null;
                currentKey = null;
                currentValue = null;
                return true;
            }
            escaped = false;
            currentValue += currentChar;
            return false;
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
                Attributes.Add(new ParsedAttribute(schema, name, name, attributeType));
            }
            else
            {
                Attributes.Add(new ParsedAttribute(schema, name, currentValue, attributeType));
            }
        }

        private void ProcessPreEquals()
        {
            if (Char.IsWhiteSpace(currentChar))
            {
                return;
            }
            if (currentChar == '=')
            {
                currentMode = Mode.PostEquals;
                return;
            }
            AddCurrent();
            currentMode = Mode.Key;
            currentKey = currentChar.ToString();

        }

        private void ProcessNull()
        {
            if (currentChar == '\\' || currentChar == '=' || currentChar == '"' || currentChar == '\'' )
            {
                throw new SystemException(string.Format("Unexpected character found. '{0}'", currentChar));
            }
            if (!Char.IsWhiteSpace(currentChar))
            {
                currentMode = Mode.Key;
                currentKey = currentChar.ToString();
            }
        }

        private void ProcessPostEquals()
        {
            if (Char.IsWhiteSpace(currentChar))
            {
                return;
            }
            currentValue = string.Empty;
            currentMode = Mode.Value;
            if (currentChar == '\'')
            {
                endSignifier = () => currentChar=='\'';
                attributeType = ParsedAttributeType.String;
                return;
            } 
            if (currentChar == '"')
            {
                endSignifier = () => currentChar== '"';
                attributeType = ParsedAttributeType.String;
                return;
            } 
            if (currentChar == '#')
            {
                var nextIndex = index + 1;
                if (attributesString.Length > nextIndex && attributesString[nextIndex] == '{')
                {
                    index = nextIndex;
                    endSignifier = () => currentChar == '}';
                    attributeType = ParsedAttributeType.Expression;
                }
                return;
            }

            endSignifier = () => Char.IsWhiteSpace(currentChar) || index == (attributesString.Length);
            attributeType = ParsedAttributeType.Reference;

            if (currentChar == '\\')
            {
                escaped = true;
            }
            else
            {
                currentValue += currentChar;
            }
        }

        private void ProcessKey()
        {
            if (currentChar == '\\' || currentChar == '"' || currentChar == '\'')
            {
                throw new SystemException(string.Format("Unexpected character found. '{0}'", currentChar));
            }
            if (Char.IsWhiteSpace(currentChar))
            {
                currentMode = Mode.PreEquals;
                return;
            }
            if (currentChar == '=')
            {
                currentMode = Mode.PostEquals;
                return;
            }
            currentKey += currentChar;
        }
    }
}