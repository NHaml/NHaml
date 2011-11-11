using NHaml.Compilers;

namespace NHaml.Rules
{
    public abstract class MarkupRule
    {
        public const string ErrorParsingTag = "Error parsing tag";
        public abstract string Signifier { get; }

        public virtual bool PerformCloseActions { get { return true; } }

        public static readonly BlockClosingAction EmptyClosingAction = () => { };
        public abstract BlockClosingAction Render(ViewSourceReader viewSourceReader, TemplateOptions options, TemplateClassBuilder builder);

        protected static void AppendText(string text, TemplateClassBuilder builder, TemplateOptions options)
        {
            var encodeHtml = options.EncodeHtml;
            if (text.StartsWith("!"))
            {
                text = text.Substring(1, text.Length - 1);
                text.TrimStart(' ');
                encodeHtml = false;
            }
            if (text.StartsWith("&"))
            {
                text = text.Substring(1, text.Length - 1);
                text.TrimStart(' ');
                encodeHtml = true;
            }

            var parser = new ExpressionStringParser(text);
            parser.Parse();
            foreach (var expressionStringToken in parser.ExpressionStringTokens)
            {
                if (expressionStringToken.IsExpression)
                {
                    builder.AppendCode(expressionStringToken.Value, encodeHtml);
                }
                else
                {
                    builder.AppendOutput(expressionStringToken.Value);
                }
            }
        }
    }
}