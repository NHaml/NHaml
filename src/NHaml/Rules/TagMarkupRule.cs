using System.Collections.Generic;
using System.Text.RegularExpressions;
using NHaml.Exceptions;
using NHaml.Properties;

namespace NHaml.Rules
{
    public class TagMarkupRule : MarkupRule
    {
        private const string Class = "class";
        private const string Id = "id";

        private static readonly Regex _idClassesRegex = new Regex(
            @"(?:(?:\#([-\w]+))|(?:\.([-\w]+)))+",
            RegexOptions.Compiled | RegexOptions.Singleline);

        private static readonly Regex _tagRegex = new Regex(
            @"^((?:[-:\w]|\\\.)+)([-\w\.\#]*)\s*(\{(.*)\})?(<)?(\/|=|&=|!=)?(.*)$",
            RegexOptions.Compiled | RegexOptions.Singleline);

        private static readonly List<string> _whitespaceSensitiveTags
            = new List<string> {"textarea", "pre"};

        public override char Signifier
        {
            get { return '%'; }
        }

        protected virtual string PreprocessLine(InputLine inputLine)
        {
            return inputLine.NormalizedText;
        }

        public override BlockClosingAction Render(TemplateParser templateParser)
        {
            var match = _tagRegex.Match(PreprocessLine(templateParser.CurrentInputLine));

            if (!match.Success)
            {
                SyntaxException.Throw(templateParser.CurrentInputLine, Resources.ErrorParsingTag,
                                      templateParser.CurrentInputLine);
            }

            var tagName = match.Groups[1].Value.Replace("\\", string.Empty);

            var isWhitespaceSensitive = _whitespaceSensitiveTags.Contains(tagName);
            var openingTag = templateParser.CurrentInputLine.Indent + '<' + tagName;
            var closingTag = "</" + tagName + '>';

            templateParser.TemplateClassBuilder.AppendOutput(openingTag);

            ParseAndRenderAttributes(templateParser, match);

            var action = match.Groups[6].Value;

            if (string.Equals("/", action)
                || templateParser.TemplateEngine.IsAutoClosingTag(tagName))
            {
                templateParser.TemplateClassBuilder.AppendOutputLine(" />");

                return null;
            }

            var content = match.Groups[7].Value.Trim();

            if (string.IsNullOrEmpty(content))
            {
                templateParser.TemplateClassBuilder.AppendOutputLine(">");
                closingTag = templateParser.CurrentInputLine.Indent + closingTag;
            }
            else
            {
                if ((content.Length > 50)
                    || string.Equals("=", action)
                    || string.Equals("&=", action)
                    || string.Equals("!=", action))
                {
                    templateParser.TemplateClassBuilder.AppendOutput(">", !isWhitespaceSensitive);

                    if (!isWhitespaceSensitive)
                    {
                        templateParser.TemplateClassBuilder.AppendOutput(templateParser.NextIndent);
                    }

                    if (string.Equals("=", action))
                    {
                        templateParser.TemplateClassBuilder.AppendCode(content, !isWhitespaceSensitive,
                                                                       templateParser.TemplateEngine.EncodeHtml);
                    }
                    else if (string.Equals("&=", action))
                    {
                        templateParser.TemplateClassBuilder.AppendCode(content, !isWhitespaceSensitive, true);
                    }
                    else if (string.Equals("!=", action))
                    {
                        templateParser.TemplateClassBuilder.AppendCode(content, !isWhitespaceSensitive, false);
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

            return () => templateParser.TemplateClassBuilder.AppendOutputLine(closingTag);
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
                var attributeParser = new AttributeParser(attributesHash);
                attributeParser.Parse();

                foreach (var attribute in attributeParser.Attributes)
                {
                    if (attribute.Type == ParsedAttributeType.String)
                    {
                        var expressionStringParser = new ExpressionStringParser(attribute.Value);

                        expressionStringParser.Parse();

                        templateParser.TemplateClassBuilder.AppendAttributeTokens(attribute.Schema, attribute.Name,
                                                                                  expressionStringParser.Tokens);
                    }
                    else
                    {
                        var token = new ExpressionStringToken(attribute.Value, true);

                        templateParser.TemplateClassBuilder.AppendAttributeTokens(attribute.Schema, attribute.Name,
                                                                                  new[] {token});
                    }
                }
            }
        }

        private static string PrependAttribute(string attributesHash, string name, string value)
        {
            var attribute = name + "=\"" + value + "\"";

            return string.IsNullOrEmpty(attributesHash) ? attribute : attribute + " " + attributesHash;
        }
    }
}