using System.Text;
using System.Text.RegularExpressions;

using NHaml.Exceptions;

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
            var currentInputLine = templateParser.CurrentInputLine;
            var match = _commentRegex.Match( currentInputLine.NormalizedText );

            if( !match.Success )
            {
                SyntaxException.Throw( currentInputLine, ErrorParsingTag, currentInputLine);
            }

            var ieBlock = match.Groups[1].Value;
            var content = match.Groups[2].Value;

            var openingTag = new StringBuilder(currentInputLine.Indent);
            openingTag.Append("<!--");
            var closingTag = new StringBuilder("-->");

            if( !string.IsNullOrEmpty( ieBlock ) )
            {
                openingTag.AppendFormat("{0}>",ieBlock);
                closingTag.Insert(0,"<![endif]");
            }

            var builder = templateParser.TemplateClassBuilder;
            if( string.IsNullOrEmpty( content ) )
            {
                builder.AppendOutputLine( openingTag.ToString() );
                closingTag.Insert(0, currentInputLine.Indent);
            }
            else
            {
                if( content.Length > 50 )
                {
                    builder.AppendOutputLine( openingTag.ToString() );
                    builder.AppendOutput( templateParser.NextIndent );
                    builder.AppendOutputLine( content );
                }
                else
                {
                    builder.AppendOutput( openingTag + content );
                    closingTag.Insert(0, ' ');
                }
            }

            return () => builder.AppendOutputLine( closingTag.ToString() );
        }
    }
}