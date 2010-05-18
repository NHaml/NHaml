using NHaml.Core.Ast;

namespace NHaml.Core.Parser.Rules
{
    public class CodeBlockMarkupRule : MarkupRuleBase
    {
        public override string[] Signifiers
        {
            get { return new[] { "-" }; }
        }

        public override AstNode Process(ParserReader parser)
        {
            var reader = parser.Input;

            reader.Skip("-");

            var code = reader.ReadToEnd();
            var node = new CodeBlockNode(code);

            if (reader.NextLine != null && reader.NextLine.Indent > reader.CurrentLine.Indent)
            {
                node.Child = parser.ParseChildren(parser.Indent, node.Child);
            }

            return node;
        }
    }
}