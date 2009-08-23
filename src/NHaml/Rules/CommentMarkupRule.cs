using System.Text;
using System.Text.RegularExpressions;
using NHaml.Compilers;
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

        public override BlockClosingAction Render(ViewSourceReader viewSourceReader, TemplateOptions options, TemplateClassBuilder builder)
        {
            var currentInputLine = viewSourceReader.CurrentInputLine;
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

            if( string.IsNullOrEmpty( content ) )
            {
                builder.AppendOutput(openingTag.ToString());
                builder.AppendOutputLine();
                closingTag.Insert(0, currentInputLine.Indent);
            }
            else
            {
                if( content.Length > 50 )
                {
                    builder.AppendOutput(openingTag.ToString());
                    builder.AppendOutputLine();

                    builder.AppendOutput( viewSourceReader.NextIndent );
                    
                    builder.AppendOutput(content);

                    builder.AppendOutputLine();
                }
                else
                {
                    builder.AppendOutput( openingTag.ToString() );
                    builder.AppendOutput( content );
                    closingTag.Insert(0, ' ');
                }
            }

            return () =>
                   {
                       builder.AppendOutput(closingTag.ToString());
                       builder.AppendOutputLine();
                   };
        }
    }
}