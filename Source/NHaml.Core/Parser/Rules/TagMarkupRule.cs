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
            {
                switch(reader.Current)
                {
                    case '#':
                    {
                        reader.Read();
                        node.Id = reader.ReadWhile(IsNameChar);
                        continue;
                    }
                    case '.':
                    {
                        reader.Read();
                        node.Class = reader.ReadWhile(IsNameChar);
                        continue;
                    }
                    case '{':
                    {
                        throw new NotSupportedException();
                    }
                    default:
                    {
                        var text = reader.Current + reader.ReadToEnd();
                        node.Chields.Add(new TextNode(text.TrimStart()));

                        break;
                    }
                }
            }

            while(parserReader.Read())
            {
                if(indent < parserReader.Indent)
                {
                    node.Chields.Add(parserReader.CreateNode());
                }
                else
                    break;
            }

            return node;
        }

        protected static bool IsNameChar(char ch)
        {
            return char.IsNumber(ch) ||
                   char.IsLetter(ch) ||
                   ch == '_' ||
                   ch == '-';
        }
    }
}