using NHaml.Core.Ast;

namespace NHaml.Core.Parser.Rules
{
    public class CodeMarkupRule : MarkupRuleBase
    {
        public override string[] Signifiers
        {
            get { return new[] { "=", "&=", "!=" }; }
        }

        public override AstNode Process(ParserReader parser)
        {
            var reader = parser.Input;

            if (!reader.Read())
                return null;

            if (reader.CurrentChar == '&')
            {
                reader.CurrentLine.EscapeLine = true;
                reader.Skip("&");
            }
            else if (reader.CurrentChar == '!')
            {
                reader.CurrentLine.EscapeLine = false;
                reader.Skip("!");
            }
            reader.Skip("=");

            var code = reader.ReadToEnd();
            var node = new CodeNode(code,reader.CurrentLine.EscapeLine);

            return node;
        }
    }
}