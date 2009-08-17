using NHaml.Compilers;

namespace NHaml.Rules
{
    public abstract class MarkupRule
    {
        public const string ErrorParsingTag = "Error parsing tag";
        public abstract string Signifier { get; }

        public virtual bool PerformCloseActions { get { return true; } }

        public static readonly BlockClosingAction EmptyClosingAction = () => { };
        public abstract BlockClosingAction Render(ViewSourceReader viewSourceReader, TemplateOptions options, TemplateClassBuilder builder);

  
    }
}