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

    public override BlockClosingAction Render(TemplateParser templateParser)
    {
      var match = _commentRegex.Match(templateParser.CurrentInputLine.NormalizedText);

      if (!match.Success)
      {
        SyntaxException.Throw(templateParser.CurrentInputLine,
          Resources.ErrorParsingTag, templateParser.CurrentInputLine);
      }

      var ieBlock = match.Groups[1].Value;
      var content = match.Groups[2].Value;

      var openingTag = templateParser.CurrentInputLine.Indent + "<!--";
      var closingTag = "-->";

      if (!string.IsNullOrEmpty(ieBlock))
      {
        openingTag += ieBlock + '>';
        closingTag = "<![endif]" + closingTag;
      }

      if (string.IsNullOrEmpty(content))
      {
        templateParser.TemplateClassBuilder.AppendOutputLine(openingTag);
        closingTag = templateParser.CurrentInputLine.Indent + closingTag;
      }
      else
      {
        if (content.Length > 50)
        {
          templateParser.TemplateClassBuilder.AppendOutputLine(openingTag);
          templateParser.TemplateClassBuilder.AppendOutput(templateParser.CurrentInputLine.Indent + "  ");
          templateParser.TemplateClassBuilder.AppendOutputLine(content);
        }
        else
        {
          templateParser.TemplateClassBuilder.AppendOutput(openingTag + content);
          closingTag = ' ' + closingTag;
        }
      }

      return () => templateParser.TemplateClassBuilder.AppendOutputLine(closingTag);
    }
  }
}