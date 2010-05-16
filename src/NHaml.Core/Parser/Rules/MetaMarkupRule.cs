using System;
using System.Collections.Generic;
using System.Text;
using NHaml.Core.Ast;

namespace NHaml.Core.Parser.Rules
{
    class MetaMarkupRule : MarkupRuleBase
    {
        DocumentNode _node;

        public MetaMarkupRule(DocumentNode node)
        {
            _node = node;
        }

        public override string[] Signifiers
        {
            get { return new[] { "@" }; }
        }

        public override AstNode Process(ParserReader parser)
        {
            var reader = parser.Input;
            if (!reader.Read())
                return null;

            reader.Skip("@");

            var name = reader.ReadName();
            string value = null;
            reader.SkipWhiteSpaces();
            switch (reader.CurrentChar)
            {
                case '=':
                    reader.Skip("=");
                    reader.SkipWhiteSpaces();
                    value = reader.ReadToEnd();
                    break;
                default:
                    return null;
            }

            List<string> data;
            if (!_node.Metadata.TryGetValue(name, out data))
            {
                data = new List<string>();
            }
            data.Add(value);
            _node.Metadata[name] = data;

            return null;
        }
    }
}
