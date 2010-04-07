using NHaml.Compilers;

namespace NHaml.Rules
{
    public class EscapeMarkupRule : MarkupRule
    {
        public override string Signifier
        {
            get { return "\\"; }
        }

        public override BlockClosingAction Render(ViewSourceReader viewSourceReader, TemplateOptions options, TemplateClassBuilder builder)
        {
            builder.AppendOutput(viewSourceReader.CurrentInputLine.Indent);
            builder.AppendOutput(viewSourceReader.CurrentInputLine.NormalizedText);
            builder.AppendOutputLine();

            return EmptyClosingAction;
        }
    }
}