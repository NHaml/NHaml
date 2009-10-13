using System;
using NHaml.Core.Ast;

namespace NHaml.Core.Parser.Rules
{
    public class TagMarkupRule : MarkupRuleBase
    {
        public override string[] Signifiers
        {
            get { return new[] {"%", ".", "#"}; }
        }

        public override AstNode Process(ParserReader parser)
        {
            var reader = parser.Input;
            var attributeParser = new AttributeParser(parser);
            var baseIndent = parser.Indent;

            if(!reader.Read())
                return null;

            var node = ReadTagNode(reader);
            
            while(!reader.IsEndOfStream)
                switch(reader.CurrentChar)
                {
                    case '#':
                    {
                        reader.Skip("#");

                        //Todo: should be a port of output
                        var attribute = node.Attributes.Find(a => a.Name.Equals("id", StringComparison.InvariantCultureIgnoreCase));

                        if(attribute == null)
                        {
                            attribute = new AttributeNode("id");
                            node.Attributes.Add(attribute);
                        }

                        attribute.Value = parser.ParseText(reader.ReadName(), reader.Index);

                        continue;
                    }
                    case '.':
                    {
                        reader.Skip(".");

                        node.Attributes.Add(new AttributeNode("class")
                        {
                            Value = parser.ParseText(reader.ReadName(), reader.Index)
                        });

                        continue;
                    }
                    case '=':
                    {
                        reader.Skip("=");

                        reader.SkipWhiteSpaces();

                        node.Child = new CodeNode(reader.ReadToEnd());

                        break;
                    }
                    case '(':
                    {
                        node.Attributes.AddRange(attributeParser.ParseHtmlStyle());
                        break;
                    }
                    case '{':
                    {
                        node.Attributes.AddRange(attributeParser.ParseRubyStyle());
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

            node.Child = parser.ParseChildren(baseIndent, node.Child);

            node.EndInfo = reader.SourceInfo;

            return node;
        }

        private static TagNode ReadTagNode(InputReader reader)
        {
            var operatorInfo = reader.SourceInfo;

            if(reader.CurrentChar == '%')
            {
                reader.Skip("%");

                var name = reader.ReadName();

                return new TagNode(name) { StartInfo = operatorInfo, OperatorInfo = operatorInfo };
            }

            return new TagNode("div") { StartInfo = operatorInfo, OperatorInfo = operatorInfo };
        }
    }
}