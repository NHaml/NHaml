using System.Text.RegularExpressions;

using NHaml.Exceptions;
using NHaml.Properties;

namespace NHaml.Rules
{
  public class CommentMarkupRule : MarkupRule
  {
    private static readonly Regex _commentRegex
      = new Regex(@"^(\[[\w\s\.]*\])?(.*)$", RegexOptions.Compiled | RegexOptions.Singleline);

    public override char Signifier
    {
      get { return '/'; }
    }

    public override bool MergeMultiline
    {
      get { return true; }
    }

    public override BlockClosingAction Render(CompilationContext compilationContext)
    {
      var match = _commentRegex.Match(compilationContext.CurrentInputLine.NormalizedText);

      if (!match.Success)
      {
        SyntaxException.Throw(compilationContext.CurrentInputLine,
          Resources.ErrorParsingTag, compilationContext.CurrentInputLine);
      }

      var ieBlock = match.Groups[1].Value;
      var content = match.Groups[2].Value;

      var openingTag = compilationContext.CurrentInputLine.Indent + "<!--";
      var closingTag = "-->";

      if (!string.IsNullOrEmpty(ieBlock))
      {
        openingTag += ieBlock + '>';
        closingTag = "<![endif]" + closingTag;
      }

      if (string.IsNullOrEmpty(content))
      {
        compilationContext.ViewBuilder.AppendOutputLine(openingTag);
        closingTag = compilationContext.CurrentInputLine.Indent + closingTag;
      }
      else
      {
        if (content.Length > 50)
        {
          compilationContext.ViewBuilder.AppendOutputLine(openingTag);
          compilationContext.ViewBuilder.AppendOutput(compilationContext.CurrentInputLine.Indent + "  ");
          compilationContext.ViewBuilder.AppendOutputLine(content);
        }
        else
        {
          compilationContext.ViewBuilder.AppendOutput(openingTag + content);
          closingTag = ' ' + closingTag;
        }
      }

      return delegate
        {
          compilationContext.ViewBuilder.AppendOutputLine(closingTag);
        };
    }
  }
}