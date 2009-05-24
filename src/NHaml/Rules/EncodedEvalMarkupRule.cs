using System.Text.RegularExpressions;

namespace NHaml.Rules
{
    public sealed class EncodedEvalMarkupRule : EvalMarkupRule
    {
        private static readonly Regex _evalRegex = new Regex(
          @"^=\s*",
          RegexOptions.Compiled | RegexOptions.Singleline );

        public override char Signifier
        {
            get { return '&'; }
        }

        public override BlockClosingAction Render( TemplateParser templateParser )
        {
            templateParser.TemplateClassBuilder.AppendOutput( templateParser.CurrentInputLine.Indent );

            var code = _evalRegex.Replace( templateParser.CurrentInputLine.NormalizedText.Trim(), string.Empty );

            templateParser.TemplateClassBuilder.AppendCodeLine( code, true );

            return EmptyClosingAction;
        }
    }
}