using NHaml.Compilers;
using NHaml.Parser;

namespace NHaml.Rules
{
    public class SilentEvalMarkupRule : MarkupRule
    {
        public const string SignifierChar = "-";

        public override string Signifier
        {
            get { return SignifierChar; }
        }

        public override BlockClosingAction Render(HamlNode hamlNode, TemplateOptions options, TemplateClassBuilder builder)
        {
            //return options.TemplateCompiler.RenderSilentEval(viewSourceReader, builder);
            return null;
        }
    }
}