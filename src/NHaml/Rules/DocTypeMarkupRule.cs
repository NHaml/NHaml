using System.Diagnostics.CodeAnalysis;

using NHaml.Exceptions;
using NHaml.Utils;

namespace NHaml.Rules
{
    public class DocTypeMarkupRule : MarkupRule
    {
        public override string Signifier
        {
            get { return "!!!"; }
        }

        [SuppressMessage( "Microsoft.Globalization", "CA1308" )]
        public override BlockClosingAction Render( TemplateParser templateParser )
        {
            var currentInputLine = templateParser.CurrentInputLine;
            var content = currentInputLine.NormalizedText.Trim().ToLowerInvariant();

            if (string.IsNullOrEmpty(content))
            {
                templateParser.TemplateClassBuilder.AppendOutputLine(
                    @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">");
            }
            else if (string.Equals(content, "1.1"))
            {
                templateParser.TemplateClassBuilder.AppendOutputLine(
                    @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.1//EN"" ""http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd"">");
            }
            else if (string.Equals(content, "strict"))
            {
                templateParser.TemplateClassBuilder.AppendOutputLine(
                    @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Strict//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd"">");
            }
            else if (string.Equals(content, "frameset"))
            {
                templateParser.TemplateClassBuilder.AppendOutputLine(
                    @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Frameset//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-frameset.dtd"">");
            }
            else if (string.Equals(content, "html"))
            {
                templateParser.TemplateClassBuilder.AppendOutputLine(
                    @"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.01 Transitional//EN"" ""http://www.w3.org/TR/html4/loose.dtd"">");
            }
            else if (string.Equals(content, "html strict"))
            {
                templateParser.TemplateClassBuilder.AppendOutputLine(
                    @"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.01//EN"" ""http://www.w3.org/TR/html4/strict.dtd"">");
            }
            else if (string.Equals(content, "html frameset"))
            {
                templateParser.TemplateClassBuilder.AppendOutputLine(
                    @"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.01 Frameset//EN"" ""http://www.w3.org/TR/html4/frameset.dtd"">");
            }
            else
            {
                var parts = content.Split(' ');

                if (string.Equals(parts[0], "xml"))
                {
                    var encoding = "utf-8";

                    if (parts.Length == 2)
                    {
                        encoding = parts[1];
                    }

                    templateParser.TemplateClassBuilder.AppendOutputLine(Utility.FormatInvariant(@"<?xml version=""1.0"" encoding=""{0}"" ?>",
                                                                                                 encoding));
                }
                else
                {
                    SyntaxException.Throw(currentInputLine, ErrorParsingTag,currentInputLine);
                }
            }


            return EmptyClosingAction;
        }
    }
}