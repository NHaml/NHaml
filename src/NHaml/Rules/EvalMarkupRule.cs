namespace NHaml.Rules
{
    public class EvalMarkupRule : MarkupRule
    {
        public override char Signifier
        {
            get { return '='; }
        }

        public override BlockClosingAction Render( TemplateParser templateParser )
        {
            templateParser.TemplateClassBuilder.AppendOutput( templateParser.CurrentInputLine.Indent );

            templateParser.TemplateClassBuilder.AppendCodeLine(
              templateParser.CurrentInputLine.NormalizedText.Trim(),
              templateParser.TemplateEngine.EncodeHtml );

            return EmptyClosingAction;
        }
    }
}