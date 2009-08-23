
using NHaml.Compilers;

namespace NHaml.Rules
{
    public  class EncodedEvalMarkupRule : EvalMarkupRule
    {

        public override string Signifier
        {
            get { return "&="; }
        }

        public override BlockClosingAction Render(ViewSourceReader viewSourceReader, TemplateOptions options, TemplateClassBuilder builder)
        {
            var inputLine = viewSourceReader.CurrentInputLine;
            builder.AppendOutput( inputLine.Indent );
            builder.AppendCode(inputLine.NormalizedText.Trim(), true);

            builder.AppendOutputLine();

            return EmptyClosingAction;
        }
    }
}