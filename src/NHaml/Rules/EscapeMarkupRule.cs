using NHaml.Compilers;

namespace NHaml.Rules
{
    public class EscapeMarkupRule : MarkupRule
    {
        public override string Signifier
        {
            get { return "\\"; }
        }

        public override BlockClosingAction Render(IViewSourceReader viewSourceReader, TemplateOptions options, TemplateClassBuilder builder)
        {
            builder.AppendOutput(viewSourceReader.CurrentInputLine.Indent);
            builder.AppendOutputLine(viewSourceReader.CurrentInputLine.NormalizedText);

            return EmptyClosingAction;
        }
    }
}