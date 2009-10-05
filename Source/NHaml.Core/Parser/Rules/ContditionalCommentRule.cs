using System;
using NHaml.Core.Ast;

namespace NHaml.Core.Parser.Rules
{
    public class ContditionalCommentRule : MarkupRuleBase
    {
        public override string[] Signifiers
        {
            get { return new[] {"/["}; }
        }

        public override AstNode Process(ParserReader parser)
        {
            var reader = new CharacterReader(parser.Text);
            reader.Read(3); // eat /[

            var node = new ConditionalCommentNode();

            node.Condition = reader.ReadWhile(c => c != ']');

            node.Child = parser.ParseChildren(parser.Indent, node.Child);

            return node;
        }
    }
}