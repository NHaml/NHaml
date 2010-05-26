using System;
using NHaml.Core.Ast;
using NHaml.Core.IO;

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
                    case '/':
                    {
                        reader.Skip("/");
                        node.AutoClose = true;
                        break;
                    }
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

                        attribute.Value = parser.ParseText(reader.ReadNameEscaped(), reader.Index);

                        continue;
                    }
                    case '.':
                    {
                        reader.Skip(".");

                        node.Attributes.Add(new AttributeNode("class")
                        {
                            Value = parser.ParseText(reader.ReadNameEscaped(), reader.Index)
                        });

                        continue;
                    }
                    case '&':
                        reader.CurrentLine.EscapeLine = true;
                        reader.Skip("&");
                        break;
                    case '!':
                        reader.CurrentLine.EscapeLine = false;
                        reader.Skip("!");
                        break;
                    case '=':
                    {
                        reader.Skip("=");

                        reader.SkipWhiteSpaces();

                        node.Child = new CodeNode(reader.ReadToEndMultiLine(), reader.CurrentLine.EscapeLine);

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
                        var text = reader.ReadToEndMultiLine();
                        node.Child = parser.ParseText(text.TrimStart(), index);

                        break;
                    }
                }

            if (reader.NextLine != null && reader.NextLine.Indent > reader.CurrentLine.Indent)
            {
                node.Child = parser.ParseChildren(baseIndent, node.Child);
            }

            node.EndInfo = reader.SourceInfo;

            return node;
        }

        private static TagNode ReadTagNode(InputReader reader)
        {
            var operatorInfo = reader.SourceInfo;

            if(reader.CurrentChar == '%')
            {
                reader.Skip("%");

                var name = reader.ReadNameEscaped();

                return new TagNode(name) { StartInfo = operatorInfo, OperatorInfo = operatorInfo };
            }

            return new TagNode("div") { StartInfo = operatorInfo, OperatorInfo = operatorInfo };
        }
    }
}