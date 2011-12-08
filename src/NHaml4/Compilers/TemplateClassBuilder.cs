using System;
using System.Collections.Generic;
using System.Text;
using NHaml;

namespace NHaml4.Compilers
{
    public abstract class TemplateClassBuilder : ITemplateClassBuilder
    {
        public const string DefaultTextWriterVariableName = "textWriter";

        protected StringBuilder Output { get; private set; }
        protected StringBuilder Preamble { get; private set; }

        public Dictionary<string, string> Meta { get; private set; }
        public string CurrentTextWriterVariableName { get; set; }
        public Type BaseType { get; set; }
        public int Depth { get; set; }
        public int BlockDepth { get; set; }
        public string ClassName { get; internal set; }

        protected TemplateClassBuilder()
        {
			CurrentTextWriterVariableName = DefaultTextWriterVariableName; 
            Output = new StringBuilder();
            Preamble = new StringBuilder();
            Meta = new Dictionary<string, string>();
        }

        public abstract void AppendOutput(string value);
        public abstract void AppendLine(string line);

        public virtual void BeginCodeBlock()
        {
            Indent();
        }

        public virtual void EndCodeBlock()
        {
            Unindent();
        }

        public void Unindent()
        {
            Depth--;
        }
        public void Indent()
        {
            Depth++;
        }

        public abstract void AppendCode(string code, bool escapeHtml);
        public abstract void AppendSilentCode(string code);
        public abstract string Build(string className);

        //public abstract void AppendPreambleCode(string code);
        //public abstract void AppendChangeOutputDepth(int depth);

        //public abstract void AppendAttributeTokens( string schema, string name, IList<ExpressionStringToken> values );
    }
}