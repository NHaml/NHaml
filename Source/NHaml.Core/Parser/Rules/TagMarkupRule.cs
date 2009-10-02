using System;
using System.Collections.Generic;
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
                        var attribute = node.GetOrAddAttribute("id");
                        attribute.Value = new TextNode(reader.ReadWhile(IsNameChar));
                        continue;
                    }
                    case '.':
                    {
                        string className = null;
                        while(reader.Current == '.')
                        {
                            reader.Read(); // eat .   

                            if(className != null)
                                className += " ";

                            className += reader.ReadWhile(IsNameChar);
                        }

                        var attribute = node.GetOrAddAttribute("class");
                        attribute.Value = new TextNode(className);

                        continue;
                    }
                    /*case '{':
                    {
                        throw new NotSupportedException();
                    }*/
                    /*case '(':
                    {
                        throw new NotSupportedException();
                    }*/
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