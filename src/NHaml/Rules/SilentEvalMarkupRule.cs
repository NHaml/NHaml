namespace NHaml.Rules
{
    public class SilentEvalMarkupRule : MarkupRule
    {
        public const string SignifierChar = "-";

        public override string Signifier
        {
            get { return SignifierChar; }
        }

        public override BlockClosingAction Render( TemplateParser templateParser )
        {
            return templateParser.TemplateEngine.Options.TemplateCompiler.RenderSilentEval( templateParser );
        }
    }
}