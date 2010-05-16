using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using NHaml.Core.Ast;

namespace NHaml.Core.Visitors
{
    public class DebugVisitor : HtmlVisitor
    {
        private TextWriter _writer;
        public Dictionary<string, string> Locals = new Dictionary<string, string>();
        private Stack<TextWriter> _stack;

        public DebugVisitor(TextWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            _writer = writer;
            _stack = new Stack<TextWriter>();
        }

        protected override void WriteCode(string code)
        {
            if (code == "1")
            {
                _writer.Write(code);
                return;
            }

            string value;
            if (Locals.TryGetValue(code, out value))
                _writer.Write(value);
            else
                _writer.Write("~~" + code + "~~");
        }

        protected override void WriteText(string text)
        {
            _writer.Write(text);
        }

        protected override void PushWriter()
        {
            _stack.Push(_writer);
            _writer = new StringWriter();
        }

        protected override string PopWriter()
        {
            if (_stack.Count==0)
                throw new InvalidOperationException("The stack is empty");
            var result = _writer.ToString();
            _writer = _stack.Pop();
            return result;
        }

        protected override void WriteStartBlock(string code, bool hasChild)
        {
            _writer.Write("~~" + code + "~~");
            if (hasChild)
                _writer.Write("~~begin~~");
        }

        protected override void WriteEndBlock()
        {
            _writer.Write("~~end~~");
        }
    }
}