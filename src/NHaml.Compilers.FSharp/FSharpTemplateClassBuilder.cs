using System;
using System.Collections.Generic;
using System.Text;
using NHaml.Utils;

namespace NHaml.Compilers.FSharp
{
    internal sealed class FSharpTemplateClassBuilder : TemplateClassBuilder
    {
        private bool _disableOutputIndentationShrink;

        public FSharpTemplateClassBuilder(string className, Type baseType)
            : base(className, baseType)
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

            value = value.Replace("\"", "\"\"");

            Output.AppendLine(string.Format("{0}textWriter.{1}(@\"{2}\")", IndentString, method, value));
        }

        public override void AppendCode(string code, bool newLine, bool escapeHtml)
        {
            if( !string.IsNullOrEmpty( code ) )
            {
                if (!code.StartsWith("\"") || !code.EndsWith("\""))
                {
                    code = string.Format("(Convert.ToString({0}))", code);
                }
                else
                {
                    code = string.Format("({0})", code);
                }

                if (escapeHtml)
                {
                    code = "(HttpUtility.HtmlEncode" + code + ")";
                }

                Output.AppendLine(IndentString + "textWriter." + (newLine ? "WriteLine" : "Write") + code);
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
                //Output.AppendLine(IndentString + "let this.Output.Depth = " + depth);
                BlockDepth = depth;
            }
        }

        public override void AppendSilentCode(string code, bool closeStatement)
        {
            if (code != null)
            {
                code = code.Trim();

                Output.AppendLine(IndentString + code);
            }
        }

        public override void AppendAttributeTokens(string schema, string name, IEnumerable<ExpressionStringToken> values)
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
            var format = string.Format("this.RenderAttributeIfValueNotNull(textWriter, \"{0}\",\"{1}\",{2})", schema,
                                       name, code);
            AppendCode(format);
        }

        public override void BeginCodeBlock()
        {
            Depth++;
            Output.AppendLine(IndentString + "(");
        }

        public override void EndCodeBlock()
        {
            Output.AppendLine(IndentString + ")");
            Depth--;
        }

        public void EndCodeBlockOnLastLine()
        {
            var linebreakLength = Environment.NewLine.Length;
            Output.Remove( Output.Length - linebreakLength, linebreakLength );
            Output.AppendLine( ")" );
            Depth--;
        }

        public override void AppendPreambleCode(string code)
        {
            Preamble.AppendLine(IndentString + code);
        }

        public override string Build()
        {
//            Output.Append( "}}" );

            var result = new StringBuilder();

            var baseClassName = Utility.MakeBaseClassName(BaseType, "<", ">", ".");
            result.Append(Utility.FormatInvariant("type {0}() as this = inherit {1}()", ClassName, baseClassName));
            if (Preamble.Length > 0 || Output.Length > 0)
            {
                result.AppendLine(" with");
                result.AppendLine("override this.CoreRender(textWriter:TextWriter) =");

                result.Append(Preamble);
                result.Append(Output);
            }
            return result.ToString();
        }
    }
}