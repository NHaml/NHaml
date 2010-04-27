using NHaml.Compilers;

namespace NHaml.Rules
{
    public class PreambleMarkupRule : SilentEvalMarkupRule
    {
        public override string Signifier
        {
            get { return "^"; }
        }
        public override BlockClosingAction Render(ViewSourceReader viewSourceReader, TemplateOptions options, TemplateClassBuilder builder)
        {
            var code = viewSourceReader.CurrentInputLine.NormalizedText.Trim();

            if( !string.IsNullOrEmpty( code ) )
            {
                builder.AppendPreambleCode( code );
            }

            return EmptyClosingAction;
        }
    }
}