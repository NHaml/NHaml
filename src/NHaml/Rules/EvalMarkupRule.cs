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
            templateParser.TemplateClassBuilder.AppendHamlComment(templateParser.CurrentInputLine.Text);
            templateParser.TemplateClassBuilder.AppendCodeLine(
              templateParser.CurrentInputLine.NormalizedText.Trim(),
              templateParser.TemplateEngine.Options.EncodeHtml );

            return EmptyClosingAction;
        }
    }
}