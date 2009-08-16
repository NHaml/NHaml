using NHaml.Compilers;

namespace NHaml.Rules
{
    public class SilentEvalMarkupRule : MarkupRule
    {
        public const string SignifierChar = "-";

        public override string Signifier
        {
            get { return SignifierChar; }
        }

        public override BlockClosingAction Render(IViewSourceReader viewSourceReader, TemplateOptions options, TemplateClassBuilder builder)
        {
            return options.TemplateCompiler.RenderSilentEval(viewSourceReader, builder);
        }
    }
}