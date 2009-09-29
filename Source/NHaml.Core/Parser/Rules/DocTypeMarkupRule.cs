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

        public override AstNode Process(ParserReader reader)
        {
            throw new NotImplementedException();
        }
    }
}