using System.Text;
using System.Text.RegularExpressions;

using NHaml.Exceptions;
using NHaml.Properties;

namespace NHaml.Rules
{
    public class CommentMarkupRule : MarkupRule
    {
        private static readonly Regex _commentRegex = new Regex(
          @"^(\[[\w\s\.]*\])?(.*)$",
          RegexOptions.Compiled | RegexOptions.Singleline );

        public override string Signifier
        {
            get { return "/"; }
        }

        public override BlockClosingAction Render( TemplateParser templateParser )
        {
            var match = _commentRegex.Match( templateParser.CurrentInputLine.NormalizedText );

            if( !match.Success )
            {
                SyntaxException.Throw( templateParser.CurrentInputLine,
                  Resources.ErrorParsingTag, templateParser.CurrentInputLine );
            }

            var ieBlock = match.Groups[1].Value;
            var content = match.Groups[2].Value;

            var openingTag = new StringBuilder(templateParser.CurrentInputLine.Indent);
            openingTag.Append("<!--");
            var closingTag = new StringBuilder("-->");

            if( !string.IsNullOrEmpty( ieBlock ) )
            {
                openingTag.AppendFormat("{0}>",ieBlock);
                closingTag.Insert(0,"<![endif]");
            }

            if( string.IsNullOrEmpty( content ) )
            {
                templateParser.TemplateClassBuilder.AppendOutputLine( openingTag.ToString() );
                closingTag.Insert(0, templateParser.CurrentInputLine.Indent);
            }
            else
            {
                if( content.Length > 50 )
                {
                    templateParser.TemplateClassBuilder.AppendOutputLine( openingTag.ToString() );
                    templateParser.TemplateClassBuilder.AppendOutput( templateParser.NextIndent );
                    templateParser.TemplateClassBuilder.AppendOutputLine( content );
                }
                else
                {
                    templateParser.TemplateClassBuilder.AppendOutput( openingTag + content );
                    closingTag.Insert(0, ' ');
                }
            }

            return () => templateParser.TemplateClassBuilder.AppendOutputLine( closingTag.ToString() );
        }
    }
}