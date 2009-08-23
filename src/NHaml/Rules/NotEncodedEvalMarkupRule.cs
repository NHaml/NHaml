
using NHaml.Compilers;

namespace NHaml.Rules
{
    public  class NotEncodedEvalMarkupRule : EvalMarkupRule
    {
        public override string Signifier
        {
            get { return "!="; }
        }
        public override BlockClosingAction Render(ViewSourceReader viewSourceReader, TemplateOptions options, TemplateClassBuilder builder)
        {
            var inputLine = viewSourceReader.CurrentInputLine;
            builder.AppendOutput( inputLine.Indent );

            builder.AppendCode( inputLine.NormalizedText.Trim(), false , false );
            builder.AppendOutputLine();
            return EmptyClosingAction;
        }
    }
}