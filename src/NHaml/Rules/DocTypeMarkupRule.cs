using System;
using System.Diagnostics.CodeAnalysis;

using NHaml.Exceptions;
using NHaml.Properties;
using NHaml.Utils;

namespace NHaml.Rules
{
  public class DocTypeMarkupRule : MarkupRule
  {
    public override char Signifier
    {
      get { return '!'; }
    }

    [SuppressMessage("Microsoft.Globalization", "CA1308")]
    public override BlockClosingAction Render(CompilationContext compilationContext)
    {
      if (compilationContext.CurrentInputLine.NormalizedText.StartsWith("!!", StringComparison.Ordinal))
      {
        var content = compilationContext.CurrentInputLine.NormalizedText.Remove(0, 2).Trim().ToLowerInvariant();

        if (string.IsNullOrEmpty(content))
        {
          compilationContext.TemplateClassBuilder.AppendOutputLine(
            @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">");
        }
        else if (string.Equals(content, "1.1"))
        {
          compilationContext.TemplateClassBuilder.AppendOutputLine(
            @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.1//EN"" ""http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd"">");
        }
        else if (string.Equals(content, "strict"))
        {
          compilationContext.TemplateClassBuilder.AppendOutputLine(
            @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Strict//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd"">");
        }
        else if (string.Equals(content, "frameset"))
        {
          compilationContext.TemplateClassBuilder.AppendOutputLine(
            @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Frameset//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-frameset.dtd"">");
        }
        else if (string.Equals(content, "html"))
        {
          compilationContext.TemplateClassBuilder.AppendOutputLine(
            @"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.01 Transitional//EN"" ""http://www.w3.org/TR/html4/loose.dtd"">");
        }
        else if (string.Equals(content, "html strict"))
        {
          compilationContext.TemplateClassBuilder.AppendOutputLine(
            @"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.01//EN"" ""http://www.w3.org/TR/html4/strict.dtd"">");
        }
        else if (string.Equals(content, "html frameset"))
        {
          compilationContext.TemplateClassBuilder.AppendOutputLine(
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

            compilationContext.TemplateClassBuilder.AppendOutputLine(Utility.FormatInvariant(@"<?xml version=""1.0"" encoding=""{0}"" ?>",
              encoding));
          }
          else
          {
            SyntaxException.Throw(compilationContext.CurrentInputLine, Resources.ErrorParsingTag,
              compilationContext.CurrentInputLine);
          }
        }
      }
      else
      {
        compilationContext.TemplateClassBuilder.AppendOutputLine(compilationContext.CurrentInputLine.Text);
      }

      return null;
    }
  }
}