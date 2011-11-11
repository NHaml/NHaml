
using NHaml.Compilers;

namespace NHaml.Rules
{
    public class EvalMarkupRule : MarkupRule
    {
        public override string Signifier
        {
            get { return "="; }
        }

        public override BlockClosingAction Render(ViewSourceReader viewSourceReader, TemplateOptions options, TemplateClassBuilder builder)
        {
            var inputLine = viewSourceReader.CurrentInputLine;

            builder.AppendOutput( inputLine.Indent );
            builder.AppendCode(inputLine.NormalizedText.Trim(), options.EncodeHtml);
            
            builder.AppendOutputLine();
            return EmptyClosingAction;
        }
    }
}