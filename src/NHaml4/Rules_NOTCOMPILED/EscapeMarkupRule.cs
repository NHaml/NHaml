using NHaml.Compilers;
using NHaml.Parser;

namespace NHaml.Rules
{
    public class EscapeMarkupRule : MarkupRule
    {
        public override string Signifier
        {
            get { return "\\"; }
        }

        public override BlockClosingAction Render(HamlNode node, TemplateOptions options, TemplateClassBuilder builder)
        {
            //builder.AppendOutput(viewSourceReader.CurrentInputLine.Indent);
            //builder.AppendOutput(viewSourceReader.CurrentInputLine.NormalizedText);
            //builder.AppendOutputLine();

            return EmptyClosingAction;
        }
    }
}