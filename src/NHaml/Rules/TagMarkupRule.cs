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
      @"\b(http|accept)\-(equiv|charset)(\s*[=:])",
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

    public override BlockClosingAction Render(TemplateParser templateParser)
    {
      var match = _tagRegex.Match(PreprocessLine(templateParser.CurrentInputLine));

      if (!match.Success)
      {
        SyntaxException.Throw(templateParser.CurrentInputLine, Resources.ErrorParsingTag,
          templateParser.CurrentInputLine);
      }

      var isWhitespaceSensitive = _whitespaceSensitiveTags.Contains(match.Groups[1].Value);

      var newLine = !templateParser.CurrentInputLine.IsMultiline;

      var openingTag = templateParser.CurrentInputLine.Indent + '<' + match.Groups[1].Value;
      var closingTag = "</" + match.Groups[1].Value + '>';

      templateParser.TemplateClassBuilder.AppendOutput(openingTag);

      ParseAndRenderAttributes(templateParser, match);

      var action = match.Groups[5].Value;

      if (string.Equals("/", action)
        || templateParser.TemplateEngine.IsAutoClosingTag(match.Groups[1].Value))
      {
        var close = " />";

        if (!newLine)
        {
          close += ' ';
        }

        templateParser.TemplateClassBuilder.AppendOutput(close, newLine);

        return null;
      }

      var content = match.Groups[6].Value.Trim();

      if (string.IsNullOrEmpty(content))
      {
        templateParser.TemplateClassBuilder.AppendOutput(">", newLine);
        closingTag = templateParser.CurrentInputLine.Indent + closingTag;
      }
      else
      {
        if ((content.Length > 50) || string.Equals("=", action))
        {
          templateParser.TemplateClassBuilder.AppendOutput(">", !isWhitespaceSensitive);

          if (!isWhitespaceSensitive)
          {
            templateParser.TemplateClassBuilder.AppendOutput(templateParser.CurrentInputLine.Indent + "  ");
          }

          if (string.Equals("=", action))
          {
            templateParser.TemplateClassBuilder.AppendCode(content, !isWhitespaceSensitive);
          }
          else
          {
            templateParser.TemplateClassBuilder.AppendOutput(content, !isWhitespaceSensitive);
          }

          if (!isWhitespaceSensitive)
          {
            closingTag = templateParser.CurrentInputLine.Indent + closingTag;
          }
        }
        else
        {
          templateParser.TemplateClassBuilder.AppendOutput(">" + content);
        }
      }

      if (!newLine)
      {
        closingTag += ' ';
      }

      return () => templateParser.TemplateClassBuilder.AppendOutput(closingTag, newLine);
    }

    private static void ParseAndRenderAttributes(TemplateParser templateParser, Match tagMatch)
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
        templateParser.TemplateClassBuilder.AppendOutput(" ");

        if (_staticAttributesRegex.IsMatch(attributesHash))
        {
          templateParser.TemplateClassBuilder.AppendOutput(_commaStripperRegex.Replace(attributesHash, "\" "));
        }
        else
        {
          var cleanAttributes = _hyphenCleanerRegex.Replace(attributesHash, "$1_$2$3");

          templateParser.TemplateEngine.TemplateCompiler.RenderAttributes(templateParser, cleanAttributes);
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