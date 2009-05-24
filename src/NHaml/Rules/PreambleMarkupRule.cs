namespace NHaml.Rules
{
    public class PreambleMarkupRule : SilentEvalMarkupRule
    {
        public override char Signifier
        {
            get { return '^'; }
        }

        public override BlockClosingAction Render( TemplateParser templateParser )
        {
            var code = templateParser.CurrentInputLine.NormalizedText.Trim();

            if( !string.IsNullOrEmpty( code ) )
            {
                templateParser.TemplateClassBuilder.AppendPreambleCode( code );
            }

            return EmptyClosingAction;
        }
    }
}