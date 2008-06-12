using System.Collections.Generic;
using System.Text.RegularExpressions;

using NHaml.Exceptions;
using NHaml.Properties;

namespace NHaml.Rules
{
  public class TagMarkupRule : MarkupRule
  {
    private const string Id = "id";
    private const string Class = "class";

    private static readonly Regex _tagRegex = new Regex(
      @"^([-:\w]+)([-\w\.\#]*)\s*(\{(.*)\})?([\/=]?)(.*)$",
      RegexOptions.Compiled | RegexOptions.Singleline);

    private static readonly Regex _idClassesRegex = new Regex(
      @"(?:(?:\#([-\w]+))|(?:\.([-\w]+)))+",
      RegexOptions.Compiled | RegexOptions.Singleline);

    private static readonly Regex _staticAttributesRegex = new Regex(
      @"^(?:[-\w]+\s*=\s*""[^""]+""\s*,?\s*)+$",
      RegexOptions.Compiled | RegexOptions.Singleline);

    private static readonly Regex _commaStripperRegex = new Regex(
      @"""\s*,\s*",
      RegexOptions.Compiled | RegexOptions.Singleline);

    private static readonly Regex _hyphenCleanerRegex = new Regex(
      @"\b(http|accept)\-(equiv|charset)(\s*=)",
      RegexOptions.Compiled | RegexOptions.Singleline);

    private static readonly Regex _keywordEscaperRegex = new Regex(
      @"(\bclass\s*=)|(\bfor\s*=)",
      RegexOptions.Compiled | RegexOptions.Singleline);

    private static readonly List<string> _whitespaceSensitiveTags
      = new List<string> {"textarea", "pre"};

    protected virtual string PreprocessLine(InputLine inputLine)
    {
      return inputLine.NormalizedText;
    }

    public override char Signifier
    {
      get { return '%'; }
    }

    public override BlockClosingAction Render(CompilationContext compilationContext)
    {
      var match = _tagRegex.Match(PreprocessLine(compilationContext.CurrentInputLine));

      if (!match.Success)
      {
        SyntaxException.Throw(compilationContext.CurrentInputLine, Resources.ErrorParsingTag,
          compilationContext.CurrentInputLine);
      }

      var isWhitespaceSensitive = _whitespaceSensitiveTags.Contains(match.Groups[1].Value);

      var newLine = !compilationContext.CurrentInputLine.IsMultiline;

      var openingTag = compilationContext.CurrentInputLine.Indent + '<' + match.Groups[1].Value;
      var closingTag = "</" + match.Groups[1].Value + '>';

      compilationContext.ViewBuilder.AppendOutput(openingTag);

      ParseAndRenderAttributes(compilationContext, match);

      var action = match.Groups[5].Value;

      if (string.Equals("/", action)
        || compilationContext.TemplateCompiler.IsAutoClosing(match.Groups[1].Value))
      {
        var close = " />";

        if (!newLine)
        {
          close += ' ';
        }

        compilationContext.ViewBuilder.AppendOutput(close, newLine);

        return null;
      }

      var content = match.Groups[6].Value.Trim();

      if (string.IsNullOrEmpty(content))
      {
        compilationContext.ViewBuilder.AppendOutput(">", newLine);
        closingTag = compilationContext.CurrentInputLine.Indent + closingTag;
      }
      else
      {
        if ((content.Length > 50) || string.Equals("=", action))
        {
          compilationContext.ViewBuilder.AppendOutput(">", !isWhitespaceSensitive);
          if (!isWhitespaceSensitive)
          {
            compilationContext.ViewBuilder.AppendOutput(compilationContext.CurrentInputLine.Indent + "  ");
          }

          if (string.Equals("=", action))
          {
            compilationContext.ViewBuilder.AppendCode(content, !isWhitespaceSensitive);
          }
          else
          {
            compilationContext.ViewBuilder.AppendOutput(content, !isWhitespaceSensitive);
          }

          if (!isWhitespaceSensitive)
          {
            closingTag = compilationContext.CurrentInputLine.Indent + closingTag;
          }
        }
        else
        {
          compilationContext.ViewBuilder.AppendOutput(">" + content);
        }
      }

      if (!newLine)
      {
        closingTag += ' ';
      }

      return () => compilationContext.ViewBuilder.AppendOutput(closingTag, newLine);
    }

    private static void ParseAndRenderAttributes(CompilationContext compilationContext, Match tagMatch)
    {
      var idAndClasses = tagMatch.Groups[2].Value;
      var attributesHash = tagMatch.Groups[4].Value.Trim();

      var match = _idClassesRegex.Match(idAndClasses);

      var classes = new List<string>();

      foreach (Capture capture in match.Groups[2].Captures)
      {
        classes.Add(capture.Value);
      }

      if (classes.Count > 0)
      {
        attributesHash = PrependAttribute(attributesHash, Class, string.Join(" ", classes.ToArray()));
      }

      string id = null;

      foreach (Capture capture in match.Groups[1].Captures)
      {
        id = capture.Value;
      }

      if (!string.IsNullOrEmpty(id))
      {
        attributesHash = PrependAttribute(attributesHash, Id, id);
      }

      if (!string.IsNullOrEmpty(attributesHash))
      {
        compilationContext.ViewBuilder.AppendOutput(" ");

        if (_staticAttributesRegex.IsMatch(attributesHash))
        {
          compilationContext.ViewBuilder.AppendOutput(_commaStripperRegex.Replace(attributesHash, "\" "));
        }
        else
        {
          compilationContext.ViewBuilder.AppendCode("new {"
            + _keywordEscaperRegex.Replace(
              _hyphenCleanerRegex.Replace(attributesHash, "$1_$2$3"), "@$1$2") +
                "}.RenderAttributes()");
        }
      }
    }

    private static string PrependAttribute(string attributesHash, string name, string value)
    {
      var attribute = name + "=\"" + value + "\"";

      return string.IsNullOrEmpty(attributesHash) ? attribute : attribute + "," + attributesHash;
    }
  }
}