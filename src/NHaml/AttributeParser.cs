using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using NHaml.Exceptions;

namespace NHaml
{
    public class AttributeParser
    {
        private static readonly Regex _escapedDoubleQuotesRegex;
        private static readonly Regex _escapedExpressionBeginQuotesRegex;
        private static readonly Regex _escapedExpressionEndQuotesRegex;
        private static readonly Regex _escapedSingleQuotesRegex;
        private static readonly Regex _parser;
        private readonly string attributesString;

        static AttributeParser()
        {
            // far more readable the a large string and 
            // since there is not dynamic code the compiler
            // can optimize it to one string.

            var pattern = string.Concat(
                @"(?:(?<schema>\w+)\:)?", // schema

                @"(?<name>[\w-]+)", // attribute name

                @"(", // start optional value for only reference

                @"\s*=\s*", // equal

                @"(?:(?<rvalue>(\.|\w)+)", // reference as value
                "|",
                @"(?:""(?<sdqvalue>((?:\\"")|[^""])*)"")", // double quotes
                "|",
                @"(?:'(?<ssqvalue>((?:\\')|[^'])*)')", // single quotes
                "|",
                @"(?:#\{(?<dvalue>((?:\\\})|[^}])*)}))", // expression
                @")?", // end optional value for only reference
                @"\s*"); // whitespace for next

            _parser = new Regex(pattern, RegexOptions.Compiled);

            _escapedDoubleQuotesRegex = new Regex(@"\\""", RegexOptions.Compiled);
            _escapedSingleQuotesRegex = new Regex(@"\\'", RegexOptions.Compiled);
            _escapedExpressionBeginQuotesRegex = new Regex(@"\\{", RegexOptions.Compiled);
            _escapedExpressionEndQuotesRegex = new Regex(@"\\}", RegexOptions.Compiled);
        }

        public AttributeParser(string attributesString)
        {
            Attributes = new List<ParsedAttribute>();
            this.attributesString = attributesString;
        }

        public List<ParsedAttribute> Attributes { get; private set; }

        public void Parse()
        {
            Attributes.Clear();

            var matches = _parser.Matches(attributesString);

            CheckMatches(matches);

            foreach (Match match in matches)
            {
                if (!match.Success)
                {
                    //Todo: put message in resource
                    throw new SyntaxException(string.Format("Attribute '{0}' is not valid.", match.Value));
                }

                var groupSchema = match.Groups["schema"];
                var groupName = match.Groups["name"];
                var groupStringDoulbeQuoteValue = match.Groups["sdqvalue"];
                var groupStringSingleQuoteValue = match.Groups["ssqvalue"];
                var groupReferenceValue = match.Groups["rvalue"];
                var groupExpressionValue = match.Groups["dvalue"];

                string schmea = null;
                var name = groupName.Value;
                string value;
                ParsedAttributeType type;

                if (groupSchema.Success)
                {
                    schmea = groupSchema.Value;
                }

                if (groupStringDoulbeQuoteValue.Success)
                {
                    type = ParsedAttributeType.String;
                    value = _escapedDoubleQuotesRegex.Replace(groupStringDoulbeQuoteValue.Value, "\"");
                }
                else if (groupStringSingleQuoteValue.Success)
                {
                    type = ParsedAttributeType.String;
                    value = _escapedSingleQuotesRegex.Replace(groupStringSingleQuoteValue.Value, "'");
                }
                else if (groupReferenceValue.Success)
                {
                    type = ParsedAttributeType.Reference;
                    value = groupReferenceValue.Value;
                }
                else if (groupExpressionValue.Success)
                {
                    type = ParsedAttributeType.Expression;
                    value = _escapedExpressionBeginQuotesRegex.Replace(groupExpressionValue.Value, "{");
                    value = _escapedExpressionEndQuotesRegex.Replace(value, "}");
                }
                else
                {
                    value = name;
                    type = ParsedAttributeType.String;
                }

                foreach (var attribute in Attributes)
                {
                    //Todo: put message in resource
                    if (string.Equals(attribute.Schema, schmea, StringComparison.InvariantCultureIgnoreCase) &&
                        string.Equals(attribute.Name, name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        var attributeName = (schmea != null ? string.Format("{0}:{1}", schmea, name) : name);
                        ThrowErrorAtPosition(string.Format("The attribute '{0}' is already defined.",attributeName),
                            groupName.Index);
                    }
                }

                Attributes.Add(new ParsedAttribute(schmea, name, value, type));
            }
        }

        private void CheckMatches(MatchCollection matches)
        {
            var currentIndex = 0;

            foreach (Match match in matches)
            {
                CheckMatch(match.Index, currentIndex);

                currentIndex = match.Index + match.Length;
            }

            if (currentIndex != attributesString.Length)
            {
                CheckMatch(attributesString.Length, currentIndex);
            }
        }

        private void CheckMatch(int matchIndex, int currentIndex)
        {
            if (matchIndex == currentIndex)
            {
                return;
            }

            var length = matchIndex - currentIndex;
            var result = attributesString.Substring(currentIndex, length).Trim();

            if (result.Length == 0)
            {
                return;
            }

            //Todo: put massage into resource
            ThrowErrorAtPosition("Invalid attribute found.", currentIndex);
        }

        private void ThrowErrorAtPosition(string message, int index)
        {
            var output = new StringBuilder();
            output.AppendLine(message);
            output.AppendLine(attributesString);
            output.Append(new string('-', index));
            output.Append('^');
            throw new SyntaxException(output.ToString());
        }
    }
}