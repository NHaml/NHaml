using System.Collections.Generic;
using System.Text.RegularExpressions;
using NHaml.Compilers;
using NHaml.Exceptions;

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
            @"^((?:[-:\w]|\\\.)+)([-\w\.\#]*)\s*(\{(.*)\})?(<)?\s*(\/|=|&=|!=)?(.*)$$",
            RegexOptions.Compiled | RegexOptions.Singleline);

        private static readonly List<string> _whitespaceSensitiveTags
            = new List<string> { "textarea", "pre" };

        public override string Signifier
        {
            get { return "%"; }
        }

        protected virtual string PreprocessLine(InputLine inputLine)
        {
            return inputLine.NormalizedText;
        }

        public override BlockClosingAction Render(ViewSourceReader viewSourceReader, TemplateOptions options, TemplateClassBuilder builder)
        {
            var currentInputLine = viewSourceReader.CurrentInputLine;
            var input = PreprocessLine(currentInputLine);
            var match = _tagRegex.Match(input);

            if (!match.Success)
            {
                SyntaxException.Throw(currentInputLine, ErrorParsingTag, currentInputLine);
            }

            var groups = match.Groups;
            var tagName = groups[1].Value.Replace("\\", string.Empty);

            var isWhitespaceSensitive = _whitespaceSensitiveTags.Contains(tagName);
            var openingTag = string.Format("{0}<{1}", currentInputLine.Indent, tagName);
            var closingTag = string.Format("</{0}>", tagName);

            builder.AppendOutput(openingTag);

            ParseAndRenderAttributes(builder, match);

            var action = groups[6].Value.Trim();

            if (string.Equals("/", action) || options.IsAutoClosingTag(tagName))
            {
                builder.AppendOutput(" />");
                builder.AppendOutputLine();
                return EmptyClosingAction;
            }

            var content = groups[7].Value.Trim();

            if (string.IsNullOrEmpty(content))
            {
                builder.AppendOutput(">");
                builder.AppendOutputLine();
                closingTag = currentInputLine.Indent + closingTag;
            }
            else
            {
                if ((content.Length > 50)
                    || string.Equals("=", action)
                    || string.Equals("&=", action)
                    || string.Equals("!=", action))
                {

                    builder.AppendOutput(">", false);

                    if (!isWhitespaceSensitive)
                    {
                        builder.AppendOutputLine();
                        builder.AppendOutput(viewSourceReader.NextIndent);
                    }

                    if (string.Equals("=", action))
                    {
                        builder.AppendCode(content, options.EncodeHtml);
                    }
                    else if (string.Equals("&=", action))
                    {
                        builder.AppendCode(content, true);
                    }
                    else if (string.Equals("!=", action))
                    {
                        builder.AppendCode(content, false);
                    }
                    else
                    {
                        builder.AppendOutput(content, false);
                    }

                    if (!isWhitespaceSensitive)
                    {
                        builder.AppendOutputLine();
                        closingTag = currentInputLine.Indent + closingTag;
                    }
                }
                else
                {
                    builder.AppendOutput(">" + content);
                    if ((currentInputLine.IndentCount + 1) == viewSourceReader.NextInputLine.IndentCount)
                    {
                        builder.AppendOutputLine();
                        closingTag = currentInputLine.Indent + closingTag;
                    }
                }
            }

            return () =>
            {
                builder.AppendOutput(closingTag);
                builder.AppendOutputLine();
            };
        }

        private static void ParseAndRenderAttributes(TemplateClassBuilder builder, Match tagMatch)
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
                break;
            }

            if (!string.IsNullOrEmpty(id))
            {
                attributesHash = PrependAttribute(attributesHash, Id, id);
            }

            if (string.IsNullOrEmpty(attributesHash))
            {
                return;
            }

            var attributeParser = new AttributeParser(attributesHash);
            attributeParser.Parse();
            foreach (var attribute in attributeParser.Attributes)
            {
                if (attribute.Type == ParsedAttributeType.String)
                {
                    var expressionStringParser = new ExpressionStringParser(attribute.Value);

                    expressionStringParser.Parse();

                    builder.AppendAttributeTokens(attribute.Schema, attribute.Name, expressionStringParser.ExpressionStringTokens);
                }
                else
                {
                    var token = new ExpressionStringToken(attribute.Value, true);

                    builder.AppendAttributeTokens(attribute.Schema, attribute.Name, new[] { token });
                }
            }
        }

        private static string PrependAttribute(string attributesHash, string name, string value)
        {
            if (string.IsNullOrEmpty(attributesHash))
            {
                return string.Format("{0}=\"{1}\"", name, value);
            }
            else
            {
                return string.Format("{0}=\"{1}\" {2}", name, value, attributesHash);
            }
        }
    }
}