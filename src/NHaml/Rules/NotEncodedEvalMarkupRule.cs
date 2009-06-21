using System.Text.RegularExpressions;

namespace NHaml.Rules
{
    public sealed class NotEncodedEvalMarkupRule : EvalMarkupRule
    {
        private static readonly Regex _evalRegex = new Regex(
          @"^=\s*",
          RegexOptions.Compiled | RegexOptions.Singleline );

        public override string Signifier
        {
            get { return "!="; }
        }

        public override BlockClosingAction Render( TemplateParser templateParser )
        {
            templateParser.TemplateClassBuilder.AppendOutput( templateParser.CurrentInputLine.Indent );

            var code = _evalRegex.Replace(templateParser.CurrentInputLine.NormalizedText.Trim(), string.Empty);

            templateParser.TemplateClassBuilder.AppendCodeLine( code, false );

            return EmptyClosingAction;
        }
    }
}