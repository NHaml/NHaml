using NHaml.Core.Ast;

namespace NHaml.Core.Parser.Rules
{
    public class CodeMarkupRule : MarkupRuleBase
    {
        public override string[] Signifiers
        {
            get { return new[] { "=" }; }
        }

        public override AstNode Process(ParserReader parser)
        {
            var reader = parser.Input;

            reader.Skip("=");

            var code = reader.ReadToEnd();
            var node = new CodeNode(code);

            return node;
        }
    }
}