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
                var ch = attributesString[index];
                switch (currentMode)
                {
                    case (null):
                        {
                            ProcessNull(ch);
                            break;
                        }
                    case (Mode.Key):
                        {
                            ProcessKey(ch);
                            break;
                        }
                    case (Mode.PreEquals):
                        {
                            ProcessPreEquals(ch);
                            break;
                        }
                    case (Mode.PostEquals):
                        {
                            ProcessPostEquals(ref index, ch);
                            break;
                        }
                    case (Mode.SingleQuoteValue):
                        {
                            ProcessValue(ch, '\'', ParsedAttributeType.String);
                            break;
                        }
                    case (Mode.DoubleQuoteValue):
                        {
                            ProcessValue(ch, '"', ParsedAttributeType.String);
                            break;
                        }
                    case (Mode.UnQuoteValue):
                        {
                            //TODO any whitespace
                            ProcessValue(ch, ' ', ParsedAttributeType.Reference);
                            break;
                        }
                    case (Mode.CodeValue):
                        {
                            ProcessValue(ch, '}', ParsedAttributeType.Expression);
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


        private void ProcessValue(char ch, char endSignifier, ParsedAttributeType type)
        {
            if (isLastChar && ch != endSignifier)
            {
                throw new SyntaxException(string.Format("Exprected '{0}' but found '{1}'.", endSignifier,ch ));
            }
            if (!escaped && ch == '\\')
            {
                escaped = true;
                return;
            }
            if (!escaped && ch == endSignifier)
            {
                AddCurrent(type);
                currentMode = null;
                currentKey = null;
                currentValue = null;
                return;
            }
            escaped = false;
            currentValue += ch;
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

        private void ProcessPreEquals(char ch)
        {
            if (Char.IsWhiteSpace(ch))
            {
                return;
            }
            if (ch == '=')
            {
                currentMode = Mode.PostEquals;
                return;
            }
            AddCurrent(ParsedAttributeType.String);
            currentMode = Mode.Key;
            currentKey = ch.ToString();

        }

        private void ProcessNull(char ch)
        {
            if (ch == '\\' || ch == '=' || ch == '"' || ch == '\'' )
            {
                throw new SystemException(string.Format("Unexpected character found. '{0}'", ch));
            }
            if (!Char.IsWhiteSpace(ch))
            {
                currentMode = Mode.Key;
                currentKey = ch.ToString();
            }
        }

        private void ProcessPostEquals(ref int index, char ch)
        {
            if (Char.IsWhiteSpace(ch))
            {
                return;
            }
            currentValue = string.Empty;
            if (ch == '\'')
            {
                currentMode = Mode.SingleQuoteValue;
                return;
            } 
            if (ch == '"')
            {
                currentMode = Mode.DoubleQuoteValue;
                return;
            } 
            if (ch == '#')
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

            if (ch == '\\')
            {
                escaped = true;
            }
            else
            {
                currentValue += ch;
            }
        }

        private void ProcessKey(char ch)
        {
            if (ch == '\\' || ch == '"' || ch == '\'')
            {
                throw new SystemException(string.Format("Unexpected character found. '{0}'", ch));
            }
            if (Char.IsWhiteSpace(ch))
            {
                currentMode = Mode.PreEquals;
                return;
            }
            if (ch == '=')
            {
                currentMode = Mode.PostEquals;
                return;
            }
            currentKey += ch;
        }
    }
}