using System.Collections.Generic;
using System.Text.RegularExpressions;
using NHaml.Exceptions;

namespace NHaml
{
    public class AttributeParser
    {
        private static readonly Regex _parser;
        private readonly string attributesString;
        private List<NHamlAttribute> _attributes = new List<NHamlAttribute>();

        static AttributeParser()
        {
            // far more readable the a large string and 
            // since there is not dynamic code the compiler
            // can optimize it to one string.

            var pattern = "";

            pattern += @"(?:(?<schema>\w):)?"; // schema

            pattern += @"(?<name>\w+)"; // attribute name

            pattern += @"("; // start optinal value for only reference

            pattern += @"\s*=\s*"; // equal

            pattern += @"(?:(?<rvalue>\w+)"; // reference as value
            pattern += "|";
            pattern += @"(?:""(?<svalue>((?:\\"")|[^""])*)"")"; // double quotes
            pattern += "|";
            pattern += @"(?:'(?<svalue>((?:\\')|[^'])*)')"; // single quotes
            pattern += "|";
            pattern += @"(?:#{(?<dvalue>[^}]*)}))"; // dynamic

            pattern += @")?"; // end optinal value for only reference

            pattern += @"\s*"; // whitespace for next

            _parser = new Regex(pattern, RegexOptions.Compiled);
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

            // ugly, but it works
            //Todo: better is to check if there are spaces between the maches and throw only them
            if( _parser.Replace(attributesString, string.Empty).Length > 0 )
                throw new SyntaxException(string.Format("Not valid attribute found: {0}", attributesString));

            foreach (Match match in _parser.Matches(attributesString))
            {
                if (!match.Success)
                    throw new SyntaxException(string.Format("Attribute '{0}' is not valid.", match.Value));

                var groupSchema = match.Groups["schema"];
                var groupName = match.Groups["name"];
                var groupStringValue = match.Groups["svalue"];
                var groupReferenceValue = match.Groups["rvalue"];
                var groupDynamicValue = match.Groups["dvalue"];

                var schmea = groupSchema != null ? groupSchema.Value : null;
                var name = groupName.Value;
                string value;
                NHamlAttributeType type;

                if( groupStringValue.Success )
                {
                    type = NHamlAttributeType.String;
                    value = groupStringValue.Value;
                }
                else if( groupReferenceValue.Success )
                {
                    type = NHamlAttributeType.Reference;
                    value = groupReferenceValue.Value;
                }
                else if( groupDynamicValue.Success )
                {
                    type = NHamlAttributeType.Dynamic;
                    value = groupDynamicValue.Value;
                }
                else
                {
                    value = name;
                    type = NHamlAttributeType.Reference;
                }

                _attributes.Add(new NHamlAttribute(schmea, name, value, type));
            }
        }
    }
}