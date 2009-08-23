using System;
using System.Collections.Generic;
using System.Text;
using NHaml.Utils;

namespace NHaml.Compilers.Boo
{
    internal  class BooTemplateClassBuilder : TemplateClassBuilder
    {
        private bool _disableOutputIndentationShrink;

        public BooTemplateClassBuilder(string className)
            : base(className)
        {
        }

        public string IndentString
        {
            get { return new string(' ', 4 + ((Depth < 0 ? 0 : Depth)*2)); }
        }

        public override void AppendOutput(string value, bool newLine)
        {
            if (value == null)
            {
                return;
            }

            var method = newLine ? "WriteLine" : "Write";

            if (!_disableOutputIndentationShrink && Depth > 0)
            {
                if (value.StartsWith(string.Empty.PadLeft(Depth*2), StringComparison.Ordinal))
                {
                    value = value.Remove(0, Depth*2);
                }
            }

            // prevents problems with " at the end of the string
            value = value.Replace("\"", "\"\"\"+'\"'+\"\"\"");

            Output.AppendLine(Utility.FormatInvariant(IndentString + "{0}.{1}(\"\"\"{2}\"\"\")", CurrentTextWriterVariableName, method, value));
        }
        public override void AppendOutputLine()
        {
            Output.AppendLine(Utility.FormatInvariant(IndentString + "{0}.WriteLine()", CurrentTextWriterVariableName));
        }

        public override void AppendCode(string code, bool escapeHtml)
        {
            if (code != null)
            {
                code = string.Format("(Convert.ToString({0}))", code);

                if (escapeHtml)
                {
                    code = string.Format("(HttpUtility.HtmlEncode{0})", code);
                }

                Output.AppendLine(string.Format("{0}textWriter.Write{1};", IndentString, code));
            }
        }

        public override void AppendChangeOutputDepth(int depth)
        {
            AppendChangeOutputDepth(depth, false);
        }

        public void AppendChangeOutputDepth(int depth, bool disableOutputIndentationShrink)
        {
            _disableOutputIndentationShrink = disableOutputIndentationShrink;

            if (BlockDepth != depth)
            {
                Output.AppendLine(string.Format("{0}Output.Depth = {1}", IndentString, depth));
                BlockDepth = depth;
            }
        }

        public override void AppendSilentCode(string code, bool closeStatement)
        {
            if (code == null)
            {
                return;
            }

            Output.Append(IndentString + code.Trim());

            if (!closeStatement && !code.EndsWith(":", StringComparison.OrdinalIgnoreCase))
            {
                Output.Append(":");
            }

            Output.AppendLine();
        }

        public override void AppendAttributeTokens(string schema, string name, IList<ExpressionStringToken> values)
        {
            var code = new StringBuilder();
            foreach (var item in values)
            {
                if (item.IsExpression)
                {
                    code.AppendFormat("({0}) + ", item.Value);
                }
                else
                {
                    code.AppendFormat("\"{0}\" + ", item.Value.Replace("\"", "\\\""));
                }
            }
            
            if (code.Length > 3)
            {
                code.Remove(code.Length - 3, 3);
            }

            var format = string.Format("RenderAttributeIfValueNotNull(textWriter, \"{0}\", \"{1}\", {2})", schema, name,
                                       code);
            AppendSilentCode(format, true);
        }


        public override void BeginCodeBlock()
        {
            Depth++;
        }

        public override void EndCodeBlock()
        {
            Depth--;
        }

        public override void AppendPreambleCode(string code)
        {
            Preamble.AppendLine(new string(' ', 4) + (code).Trim());
        }

        public override string Build(IList<string> imports)
        {
            var result = new StringBuilder();

            result.AppendLine(Utility.FormatInvariant("class {0}({1}):",
                                                      ClassName, Utility.MakeBaseClassName(BaseType, "[of ", "]", ".")));
            result.AppendLine("  override def CoreRender(textWriter as System.IO.TextWriter):");

            result.Append(Preamble);
            result.Append(Output);

            return result.ToString();
        }
    }
}