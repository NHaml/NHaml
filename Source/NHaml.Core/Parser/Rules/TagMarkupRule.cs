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

        public override AstNode Process(ParserReader parserReader)
        {
            var reader = new CharacterReader(parserReader.Text);
            var indent = parserReader.Indent;
            TagNode node;

            if(!reader.Read())
                return null;

            if(reader.Current == '%')
            {
                reader.Read(); // eat %

                var name = reader.ReadWhile(IsNameChar);

                node = new TagNode(name);
            }
            else
                node = new TagNode("div");

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

                        attribute.Value = new TextNode(reader.ReadWhile(IsNameChar));

                        continue;
                    }
                    case '.':
                    {
                        reader.Read(); // eat .

                        node.Attributes.Add(new AttributeNode("class")
                        {
                            Value = new TextNode(reader.ReadWhile(IsNameChar))
                        });

                        continue;
                    }
                        /*case '{':
                    {
                        throw new NotSupportedException();
                    }*/
                    case '(':
                    {
                        reader.Read(); // eat (

                        while(reader.Current != ')')
                        {
                            reader.ReadWhile(c => char.IsWhiteSpace(c));

                            if(reader.IsEndOfStream)
                            {
                                if(!parserReader.Read())
                                    break;
                                
                                reader = new CharacterReader(parserReader.Text);
                                reader.Read();
                            }

                            var name = reader.ReadWhile(IsNameChar);

                            reader.ReadWhile(c => char.IsWhiteSpace(c));

                            //Todo: report error when there is no =
                            reader.Read(); // =

                            reader.ReadWhile(c => char.IsWhiteSpace(c));

                            var attribute = new AttributeNode(name);
                            node.Attributes.Add(attribute);
                            switch(reader.Current)
                            {
                                case '\'':
                                {
                                    reader.Read(); // skip '
                                    attribute.Value = new TextNode(reader.ReadWhile(c => c != '\''));
                                    reader.Read(); // skip '
                                    break;
                                }

                                default:
                                {
                                    attribute.Value = new LocalNode(reader.ReadWhile(IsNameChar));
                                    break;
                                }
                            }
                        }

                        break;
                    }
                    default:
                    {
                        var text = reader.Current + reader.ReadToEnd();
                        node.Child = new TextNode(text.TrimStart()) {IsInline = true};

                        break;
                    }
                }

            node.Child = parserReader.ParseChildren(indent, node.Child);

            return node;
        }
    }
}