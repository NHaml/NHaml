using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NHaml
{
    public class AttributeParser
    {
        private readonly string attributes;
        private enum State
        {
            Code,
            Text,
            Key
        }

        private State? state;
        private string currentValue;
        private string currentKey;
        private bool isCurrentCode;
        private bool isSingleQuotes;
        private int index;
        public static Regex StartCodeRegex;
        public static Regex StartTextWithDoubleQuotesRegex;
        public static Regex StartTextWithSingleQuotesRegex;
        public static Regex StartTextWithOutQuotesRegex;
        public static Regex KeyValueSplitRegex;


        static AttributeParser()
        {
            StartCodeRegex = new Regex(@"[\w:]*(?<ToTrim>\s*=\s*\#{)$");
            KeyValueSplitRegex = new Regex(@"[\w:]*(?<ToTrim>\s*,\s*)$");
            StartTextWithDoubleQuotesRegex = new Regex("[\\w:]*(?<ToTrim>\\s*=\\s*\")$");
            StartTextWithSingleQuotesRegex = new Regex("[\\w:]*(?<ToTrim>\\s*=\\s*\')$");
            StartTextWithOutQuotesRegex = new Regex(@"[\w:]*(?<ToTrim>\s*=.*)$");
        }

        // So at the moment, I'm leaning towards {foo="bar"} or {foo=bar} for
        // static attributes and {foo=#{bar}} for dynamic ones. Thoughts?
        public AttributeParser(string attributes)
        {
            this.attributes = attributes.TrimStart();
            Values = new List<KeyValuePair>();
        }
        public void Parse()
        {
            for (index = attributes.Length - 1; index > -1; )
            {
                var character = attributes[index];
                if (state == null)
                {
                    currentValue = null;
                    currentKey = null;
                    if (character == '}')
                    {
                        state = State.Code;
                        isCurrentCode = true;
                    }
                    else if (character != ' ' && character != '\t')
                    {
                        if (character == '"')
                        {
                            isSingleQuotes = false;
                        }
                        else if (character == '\'')
                        {
                            isSingleQuotes = true;
                        }
                        state = State.Text;
                        isCurrentCode = false;
                    }
                    index--;
                }
                else
                {
                    switch (state.Value)
                    {
                        case (State.Code):
                            {
                                ProcessCodeChar(character);
                                break;
                            }
                        case (State.Key):
                            {
                                ProcessKeyChar(character);
                                break;
                            }
                        case (State.Text):
                            {
                                ProcessTextChar(character);
                                break;
                            }
                    }
                }
            }
        }

        private void ProcessTextChar(char character)
        {

            var substring = attributes.Substring(0, index + 1);
            Regex regex;
            if (isSingleQuotes)
            {
                regex = StartTextWithSingleQuotesRegex;
            }
            else
            {
                regex = StartTextWithDoubleQuotesRegex;
            }
            var group = regex.Match(substring).Groups["ToTrim"];
            if (group.Success)
            {
                index = index - group.Value.Length;
                state = State.Key;
                return;
            }

            currentValue = character + currentValue;
            index--;
        }

        private void ProcessKeyChar(char character)
        {

            var substring = attributes.Substring(0, index + 1);
            var group = KeyValueSplitRegex.Match(substring).Groups["ToTrim"];
            if (group.Success)
            {
                AddCurrentToValues();
                state = null;
                index = index - group.Value.Length;
                return;
            }

            currentKey = character + currentKey;
            if (index == 0)
            {
                AddCurrentToValues();
                state = null;
                return;
            }
            index--;
        }

        private void AddCurrentToValues()
        {
            Values.Insert(0, new KeyValuePair
            {
                IsCode = isCurrentCode,
                Key = currentKey,
                Value = currentValue
            });
        }

        private void ProcessCodeChar(char character)
        {
            var substring = attributes.Substring(0, index + 1);
            var group = StartCodeRegex.Match(substring).Groups["ToTrim"];
            if (group.Success)
            {
                index = index - group.Value.Length;
                state = State.Key;
                return;
            }

            currentValue = character + currentValue;
            index--;
        }




        public List<KeyValuePair> Values { get; set; }
        public class KeyValuePair
        {
            public string Key;
            public string Value;
            public bool IsCode;
        }
    }
}
