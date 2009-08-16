
using NHaml.Compilers;

namespace NHaml.Rules
{
    public sealed class EncodedEvalMarkupRule : EvalMarkupRule
    {

        public override string Signifier
        {
            get { return "&="; }
        }

        public override BlockClosingAction Render(IViewSourceReader viewSourceReader, TemplateOptions options, TemplateClassBuilder builder)
        {
            var inputLine = viewSourceReader.CurrentInputLine;
            builder.AppendOutput( inputLine.Indent );
            builder.AppendCodeLine( inputLine.NormalizedText.Trim(), true );
            return EmptyClosingAction;
        }
    }
}