
using NHaml.Compilers;
using NHaml.Parser;

namespace NHaml.Rules
{
    public class MetaMarkupRule : MarkupRule
    {

        public override string Signifier
        {
            get { return "@"; }
        }

        public override BlockClosingAction Render(HamlNode node, TemplateOptions options, TemplateClassBuilder builder)
        {
            //var content = viewSourceReader.CurrentInputLine.NormalizedText.Trim().Replace( "\"", "\"\"" );

            //var indexOfEquals = content.IndexOf('=');
            //var key = content.Substring(0, indexOfEquals).Trim();
            //var value = content.Substring(indexOfEquals+1, content.Length - indexOfEquals - 1).Trim();

            //builder.Meta[key] = value;

            return EmptyClosingAction;
        }
    }
}