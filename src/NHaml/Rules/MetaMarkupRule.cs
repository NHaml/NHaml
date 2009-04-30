using System.Text.RegularExpressions;

namespace NHaml.Rules
{
    public class MetaMarkupRule : MarkupRule
    {
        private static readonly Regex _pairRegex = new Regex( @"^\s*(\w*)\s*=\s*(.*)\s*$",
            RegexOptions.Compiled | RegexOptions.Singleline );

        public override char Signifier
        {
            get { return '@'; }
        }

        public override BlockClosingAction Render( TemplateParser templateParser )
        {
            var content = templateParser.CurrentInputLine.NormalizedText.Trim().Replace( "\"", "\"\"" );

            var match = _pairRegex.Match( content );

            templateParser.Meta[match.Groups[1].Value] = match.Groups[2].Value;

            return null;
        }
    }
}