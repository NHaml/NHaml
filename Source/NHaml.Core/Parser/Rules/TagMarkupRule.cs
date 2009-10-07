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
            var reader = new CharacterReader(parser.Text, 0);
            var attributeParser = new AttributeParser(reader, parser);
            var baseIndent = parser.Indent;

            if(!reader.Read()) // initial read
                return null;

            var node = ReadTagNode(reader);

            while(!reader.IsEndOfStream)
                switch(reader.Current)
                {
                    case '#':
                    {
                        reader.Read(); // eat #

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
                        reader.Read(); // eat .

                        node.Attributes.Add(new AttributeNode("class")
                        {
                            Value = parser.ParseText(reader.ReadName(), reader.Index)
                        });

                        continue;
                    }
                    case '=':
                    {
                        reader.Read(); // eat =

                        reader.ReadWhiteSpaces();

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
                        node.Child = parser.ParseText(text.TrimStart(),index);

                        break;
                    }
                }

            node.Child = parser.ParseChildren(baseIndent, node.Child);

            return node;
        }

        private static TagNode ReadTagNode(CharacterReader reader)
        {
            if(reader.Current == '%')
            {
                reader.Read(); // eat %

                var name = reader.ReadName();

                return new TagNode(name);
            }

            return new TagNode("div");
        }
    }
}