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
            var attributeParser = new AttributeParser(parser);
            var baseIndent = parser.Indent;

            if (!reader.Read())
                return null;

            reader.Skip("@");
            var name = reader.ReadName();
            var node = new MetaNode(name);

            reader.SkipWhiteSpaces();

            while (!reader.IsEndOfStream)
            {
                switch (reader.CurrentChar)
                {
                    case '=':
                        {
                            reader.Skip("=");
                            reader.SkipWhiteSpaces();
                            switch (reader.CurrentChar) {
                                case '\'':
                                    reader.Skip("'");
                                    node.Value = reader.ReadWhile(c => c != '\'');
                                    reader.Skip("'");
                                    break;
                                case '"':
                                    reader.Skip("\"");
                                    node.Value = reader.ReadWhile(c => c != '"');
                                    reader.Skip("\"");
                                    break;
                                default:
                                    node.Value = reader.ReadName();
                                    break;
                            }
                            break;
                        }
                    case '(':
                        {
                            node.Attributes.AddRange(attributeParser.ParseHtmlStyle(false));
                            break;
                        }
                    case '{':
                        {
                            node.Attributes.AddRange(attributeParser.ParseRubyStyle(false));
                            break;
                        }
                    default:
                        {
                            var index = reader.Index;
                            var text = reader.ReadToEnd();
                            node.Child = parser.ParseText(text.TrimStart(), index);
                            break;
                        }
                }
                reader.SkipWhiteSpaces();
            }

            if (reader.NextLine != null && reader.NextLine.Indent > reader.CurrentLine.Indent)
            {
                node.Child = parser.ParseChildren(baseIndent, node.Child);
            }

            node.EndInfo = reader.SourceInfo;
            if (!_node.Metadata.ContainsKey(name)) _node.Metadata[name] = new List<MetaNode>();
            _node.Metadata[name].Add(node);
            return node;
        }
    }
}
