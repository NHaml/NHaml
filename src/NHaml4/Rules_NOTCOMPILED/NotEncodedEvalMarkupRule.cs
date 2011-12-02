
using NHaml.Compilers;
using NHaml.Parser;

namespace NHaml.Rules
{
    public  class NotEncodedEvalMarkupRule : EvalMarkupRule
    {
        public override string Signifier
        {
            get { return "!="; }
        }
        public override BlockClosingAction Render(HamlNode node, TemplateOptions options, TemplateClassBuilder builder)
        {
            //var inputLine = viewSourceReader.CurrentInputLine;
            //builder.AppendOutput( inputLine.Indent );

            //builder.AppendCode( inputLine.NormalizedText.Trim(), false );
            //builder.AppendOutputLine();
            return EmptyClosingAction;
        }
    }
}