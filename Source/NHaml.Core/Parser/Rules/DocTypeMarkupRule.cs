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
            var reader = parser.Input;

            reader.Skip("!!!");

            var node = new DocTypeNode();

            if(!reader.IsEndOfStream)
            {
                reader.SkipWhiteSpaces();

                node.Text = reader.ReadToEnd();
            }

            return node;
        }
    }
}