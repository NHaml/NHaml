
namespace NHaml.Rules
{
    public class EvalMarkupRule : MarkupRule
    {
        public override string Signifier
        {
            get { return "="; }
        }

        public override BlockClosingAction Render( TemplateParser templateParser )
        {
            var builder = templateParser.TemplateClassBuilder;
            var inputLine = templateParser.CurrentInputLine;

            builder.AppendOutput( inputLine.Indent );
            builder.AppendHamlComment(inputLine.Text);
            builder.AppendCodeLine(inputLine.NormalizedText.Trim(), templateParser.TemplateEngine.Options.EncodeHtml );

            return EmptyClosingAction;
        }
    }
}