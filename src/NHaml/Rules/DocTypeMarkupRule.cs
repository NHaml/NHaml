using System.Diagnostics.CodeAnalysis;
using NHaml.Compilers;
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
        public override BlockClosingAction Render(ViewSourceReader viewSourceReader, TemplateOptions options, TemplateClassBuilder builder)
        {
            var currentInputLine = viewSourceReader.CurrentInputLine;
            var content = currentInputLine.NormalizedText.Trim().ToLowerInvariant();

            if (string.IsNullOrEmpty(content))
            {
                builder.AppendOutput(@"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">");
            }
            else if (string.Equals(content, "1.1"))
            {
                builder.AppendOutput(@"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.1//EN"" ""http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd"">");
            }
            else if (string.Equals(content, "strict"))
            {
                builder.AppendOutput(@"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Strict//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd"">");
            }
            else if (string.Equals(content, "frameset"))
            {
                builder.AppendOutput(@"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Frameset//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-frameset.dtd"">");
            }
            else if (string.Equals(content, "html"))
            {
                builder.AppendOutput(@"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.01 Transitional//EN"" ""http://www.w3.org/TR/html4/loose.dtd"">");
            }
            else if (string.Equals(content, "html strict"))
            {
                builder.AppendOutput(@"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.01//EN"" ""http://www.w3.org/TR/html4/strict.dtd"">");
            }
            else if (string.Equals(content, "html frameset"))
            {
                builder.AppendOutput(@"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.01 Frameset//EN"" ""http://www.w3.org/TR/html4/frameset.dtd"">");
            }
            else if (string.Equals(content, "mobile"))
            {
                builder.AppendOutput("<!DOCTYPE html PUBLIC \"-//WAPFORUM//DTD XHTML Mobile 1.2//EN\" \"http://www.openmobilealliance.org/tech/DTD/xhtml-mobile12.dtd\">");
            }
            else if (string.Equals(content, "basic"))
            {
                builder.AppendOutput("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML Basic 1.1//EN\" \"http://www.w3.org/TR/xhtml-basic/xhtml-basic11.dtd\">");
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

                    var invariant = Utility.FormatInvariant(@"<?xml version=""1.0"" encoding=""{0}"" ?>", encoding);
                    builder.AppendOutput(invariant);
                }
                else
                {
                    SyntaxException.Throw(currentInputLine, ErrorParsingTag,currentInputLine);
                }
            }

            builder.AppendOutputLine();

            return EmptyClosingAction;
        }
    }
}