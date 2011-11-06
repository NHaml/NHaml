using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using NHaml.Core.Ast;
using NHaml.Core.Visitors;

namespace NHaml.Core.Tests
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

        protected override void WriteCode(string code, bool escapteHtml)
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

        protected override object PopWriter()
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

        protected override void WriteData(object data, string filter)
        {
            if (filter == null)
            {
                WriteText(data as string);
                return;
            }
            switch (filter) {
                case "preserve":
                    {
                        var replace = data as string;
                        replace += System.Environment.NewLine;
                        WriteText(replace.Replace(System.Environment.NewLine, "&#x000A;"));
                        break;
                    }
                case "plain":
                    {
                        WriteText(data as string);
                        break;
                    }
                case "escaped":
                    {
                        var text = data as string;
                        WriteText(HttpUtility.HtmlEncode(text));
                        break;
                    }
                default:
                    throw new NotSupportedException();
            }
        }

        protected override LateBindingNode DataJoiner(string joinString, object[] data, bool sort)
        {
            List<string> d = new List<string>();
            foreach (object o in data)
            {
                d.Add(o as string);
            }
            if (sort)
            {
                d.Sort();
            }
            return new LateBindingNode() { Value = String.Join(joinString, d.ToArray()) };
        }
    }
}