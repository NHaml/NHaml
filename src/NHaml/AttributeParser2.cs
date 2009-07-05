using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            SingleQuoteValue,
            DoubleQuoteValue,
            UnQuoteValue,
            CodeValue
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
        private bool isLastChar; 
        char currentChar;


        public List<ParsedAttribute> Attributes { get; private set; }

        //modes 
        //none
        //value startvaluematch
        //key 
        //equals
        public void Parse()
        {
            Attributes.Clear();

            
            for (var index = 0; index < attributesString.Length; index++)
            {
                isLastChar = index == (attributesString.Length - 1);
                Debug.WriteLine(isLastChar);
                
                currentChar = attributesString[index];
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
                            ProcessPostEquals(ref index);
                            break;
                        }
                    case (Mode.SingleQuoteValue):
                        {
                            ProcessValue( '\'', ParsedAttributeType.String);
                            break;
                        }
                    case (Mode.DoubleQuoteValue):
                        {
                            ProcessValue( '"', ParsedAttributeType.String);
                            break;
                        }
                    case (Mode.UnQuoteValue):
                        {
                            //TODO any whitespace
                            ProcessValue( ' ', ParsedAttributeType.Reference);
                            break;
                        }
                    case (Mode.CodeValue):
                        {
                            ProcessValue( '}', ParsedAttributeType.Expression);
                            break;
                        }
                    default:
                        {
                            throw new NotImplementedException();
                        }

                }
            }
            if (currentMode != null)
            {
                throw new SyntaxException();
            }
           

        }


        private void ProcessValue( char endSignifier, ParsedAttributeType type)
        {
            if (isLastChar && currentChar != endSignifier)
            {
                throw new SyntaxException(string.Format("Exprected '{0}' but found '{1}'.", endSignifier,currentChar ));
            }
            if (!escaped && currentChar == '\\')
            {
                escaped = true;
                return;
            }
            if (!escaped && currentChar == endSignifier)
            {
                AddCurrent(type);
                currentMode = null;
                currentKey = null;
                currentValue = null;
                return;
            }
            escaped = false;
            currentValue += currentChar;
        }

        private void AddCurrent(ParsedAttributeType type)
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
                    break;
                default:
                    throw new SystemException("only 1 ':' allowed in key");
            }

            if (currentValue == null)
            {
                Attributes.Add(new ParsedAttribute(schema, name, name, type));
            }
            else
            {
                Attributes.Add(new ParsedAttribute(schema, name, currentValue, type));
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
            AddCurrent(ParsedAttributeType.String);
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

        private void ProcessPostEquals(ref int index)
        {
            if (Char.IsWhiteSpace(currentChar))
            {
                return;
            }
            currentValue = string.Empty;
            if (currentChar == '\'')
            {
                currentMode = Mode.SingleQuoteValue;
                return;
            } 
            if (currentChar == '"')
            {
                currentMode = Mode.DoubleQuoteValue;
                return;
            } 
            if (currentChar == '#')
            {
                var nextIndex = index + 1;
                if (attributesString.Length > nextIndex && attributesString[nextIndex] == '{')
                {
                    index = nextIndex;
                    currentMode = Mode.CodeValue;
                }
                return;
            }
            currentMode = Mode.UnQuoteValue;

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