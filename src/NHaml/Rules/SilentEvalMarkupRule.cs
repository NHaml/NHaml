namespace NHaml.Rules
{
    public class SilentEvalMarkupRule : MarkupRule
    {
        public const char SignifierChar = '-';

        public override char Signifier
        {
            get { return SignifierChar; }
        }

        public override BlockClosingAction Render( TemplateParser templateParser )
        {
            templateParser.TemplateClassBuilder.AppendHamlComment(templateParser.CurrentInputLine.Text);
            return templateParser.TemplateEngine.Options.TemplateCompiler.RenderSilentEval( templateParser );
        }
    }
}