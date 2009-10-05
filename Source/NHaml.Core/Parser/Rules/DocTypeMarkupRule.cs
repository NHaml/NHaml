using System;
using NHaml.Core.Ast;

namespace NHaml.Core.Parser.Rules
{
    public class DocTypeMarkupRule : MarkupRuleBase
    {
        public override string[] Signifiers
        {
            get { return new[] {"!!!"}; }
        }

        public override AstNode Process(ParserReader parser)
        {
            var reader = new CharacterReader(parser.Text);

            reader.Read(4); // skip !!!

            var node = new DocTypeNode();

            if(!reader.IsEndOfStream)
            {
                reader.ReadWhile(c => char.IsWhiteSpace(c));

                node.Text = reader.ReadToEnd();
            }

            return node;
        }
    }
}