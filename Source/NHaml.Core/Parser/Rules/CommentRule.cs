using System;
using System.Collections.Generic;
using NHaml.Core.Ast;

namespace NHaml.Core.Parser.Rules
{
    public class CommentRule : MarkupRuleBase
    {
        public override string[] Signifiers
        {
            get { return new[] { "/", "-#" }; }
        }

        public override AstNode Process(ParserReader parser)
        {
            if(parser.Text.StartsWith("-#"))
            {
                parser.ParseChildren(parser.Indent, null);
                return null;
            }

            var node = new CommentNode();
            var reader = new CharacterReader(parser.Text);
            reader.Read();
            reader.Read(); // eat /

            reader.ReadWhile(c=>char.IsWhiteSpace(c));

            if(!reader.IsEndOfStream)
            {
                node.Child = new TextNode(reader.ReadWhile(IsNameChar));
            }

            node.Child = parser.ParseChildren(parser.Indent, node.Child);

            return node;
        }
    }
}