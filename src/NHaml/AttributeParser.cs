using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using NHaml.Exceptions;

namespace NHaml
{
    public class AttributeParser
    {
        private static readonly Regex _parser;
        private static readonly Regex _escapedDoubleQuotesRegex;
        private static readonly Regex _escapedSingleQuotesRegex;
        private static readonly Regex _escapedExpressionBeginQuotesRegex;
        private static readonly Regex _escapedExpressionEndQuotesRegex;
        private readonly string attributesString;
        private List<NHamlAttribute> _attributes = new List<NHamlAttribute>();

        static AttributeParser()
        {
            // far more readable the a large string and 
            // since there is not dynamic code the compiler
            // can optimize it to one string.

            var pattern = "";

            pattern += @"(?:(?<schema>\w+)\:)?"; // schema

            pattern += @"(?<name>\w+)"; // attribute name

            pattern += @"("; // start optinal value for only reference

            pattern += @"\s*=\s*"; // equal

            pattern += @"(?:(?<rvalue>(\.|\w)+)"; // reference as value
            pattern += "|";
            pattern += @"(?:""(?<sdqvalue>((?:\\"")|[^""])*)"")"; // double quotes
            pattern += "|";
            pattern += @"(?:'(?<ssqvalue>((?:\\')|[^'])*)')"; // single quotes
            pattern += "|";
            pattern += @"(?:#\{(?<dvalue>((?:\\\})|[^}])*)}))"; // expression

            pattern += @")?"; // end optinal value for only reference

            pattern += @"\s*"; // whitespace for next

            _parser = new Regex(pattern, RegexOptions.Compiled);

            _escapedDoubleQuotesRegex = new Regex( @"\\""", RegexOptions.Compiled );
            _escapedSingleQuotesRegex = new Regex( @"\\'", RegexOptions.Compiled );
            _escapedExpressionBeginQuotesRegex = new Regex( @"\\{", RegexOptions.Compiled );
            _escapedExpressionEndQuotesRegex = new Regex( @"\\}", RegexOptions.Compiled );
        }

        public AttributeParser(string attributesString)
        {
            this.attributesString = attributesString;
        }

        public ICollection<NHamlAttribute> Attributes
        {
            get { return _attributes; }
        }

        public void Parse()
        {
            _attributes.Clear();

            var matches = _parser.Matches(attributesString);

            CheckMatches(matches);

            foreach (Match match in matches)
            {
                if (!match.Success)
                    //Todo: put message in resource
                    throw new SyntaxException(string.Format("Attribute '{0}' is not valid.", match.Value));

                var groupSchema = match.Groups["schema"];
                var groupName = match.Groups["name"];
                var groupStringDoulbeQuoteValue = match.Groups["sdqvalue"];
                var groupStringSingleQuoteValue = match.Groups["ssqvalue"];
                var groupReferenceValue = match.Groups["rvalue"];
                var groupExpressionValue = match.Groups["dvalue"];

                var schmea = groupSchema.Success ? groupSchema.Value : null;
                var name = groupName.Value;
                string value;
                NHamlAttributeType type;

                if (groupStringDoulbeQuoteValue.Success)
                {
                    type = NHamlAttributeType.String;
                    value = _escapedDoubleQuotesRegex.Replace(groupStringDoulbeQuoteValue.Value, "\"");
                }
                else if( groupStringSingleQuoteValue.Success )
                {
                    type = NHamlAttributeType.String;
                    value = _escapedSingleQuotesRegex.Replace(groupStringSingleQuoteValue.Value, "'");
                }
                else if( groupReferenceValue.Success )
                {
                    type = NHamlAttributeType.Reference;
                    value = groupReferenceValue.Value;
                }
                else if (groupExpressionValue.Success)
                {
                    type = NHamlAttributeType.Expression;
                    value = _escapedExpressionBeginQuotesRegex.Replace(groupExpressionValue.Value, "{");
                    value = _escapedExpressionEndQuotesRegex.Replace(value, "}");
                }
                else
                {
                    value = name;
                    type = NHamlAttributeType.Reference;
                }

                _attributes.Add(new NHamlAttribute(schmea, name, value, type));
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
                return;

            var length = matchIndex - currentIndex;
            var result = attributesString.Substring(currentIndex, length).Trim();

            if (result.Length == 0)
                return;

            //Todo: put massage into resource
            var message = new StringBuilder();
            message.AppendLine( "Error while parsing attribute:" );
            message.AppendLine( attributesString );
            message.Append( new string( ' ', currentIndex ) );
            message.Append( new string( '^', length ) );
            throw new SyntaxException( message.ToString() );
        }
    }
}