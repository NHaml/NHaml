using System;
using NHaml.Core.Ast;
using NHaml.Core.IO;

namespace NHaml.Core.Parser.Rules
{
    public class EscapedTextMarkupRule : MarkupRuleBase
    {
        public override string[] Signifiers
        {
            get { return new[] {"!", "&", "\\"}; }
        }

        public override AstNode Process(ParserReader parser)
        {
            var reader = parser.Input;

            if(!reader.Read())
                return null;

            if (reader.CurrentChar == '!')
            {
                reader.CurrentLine.EscapeLine = false;
                reader.Skip("!");
            }
            else if (reader.CurrentChar == '&')
            {
                reader.CurrentLine.EscapeLine = true;
                reader.Skip("&");
            }
            else if (reader.CurrentChar == '\\')
            {
                reader.Skip("\\");
            }

            var index = reader.Index;
            var text = reader.ReadToEndMultiLine();
            return parser.ParseText(text.TrimStart(), index);
        }
    }
}