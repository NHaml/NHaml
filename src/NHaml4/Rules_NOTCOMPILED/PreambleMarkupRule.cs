using NHaml.Compilers;
using NHaml.Parser;

namespace NHaml.Rules
{
    public class PreambleMarkupRule : SilentEvalMarkupRule
    {
        public override string Signifier
        {
            get { return "^"; }
        }
        public override BlockClosingAction Render(HamlNode node, TemplateOptions options, TemplateClassBuilder builder)
        {
            //var code = viewSourceReader.CurrentInputLine.NormalizedText.Trim();

            //if( !string.IsNullOrEmpty( code ) )
            //{
            //    builder.AppendPreambleCode( code );
            //}

            return EmptyClosingAction;
        }
    }
}