using System;
using System.Collections.Generic;
using System.Text;
using NHaml.Core.Ast;

namespace NHaml.Core.Visitors
{
    public class WebFormsVisitor : HtmlVisitor
    {
        StringBuilder _sb;
        Stack<StringBuilder> _stack;

        public WebFormsVisitor()
        {
            _sb = new StringBuilder();
            _stack = new Stack<StringBuilder>();
        }

        public override void Visit(DocumentNode node)
        {
            List<string> data;
            if (node.Metadata.TryGetValue("namespace",out data)) {
                foreach (string str in data) {
                    WriteText("<%@ Import Namespace=\""+str+"\" %>" + Environment.NewLine);
                }
            }

            if (node.Metadata.TryGetValue("assembly",out data)) {
                foreach (string str in data) {
                    WriteText("<%@ Assembly Name=\"" + str + "\" %>" + Environment.NewLine);
                }
            }

            bool isPage = true;
            Dictionary<string,string> d = new Dictionary<string,string>();

            if (node.Metadata.TryGetValue("control",out data)) {
                isPage = false;
            }

            if (node.Metadata.TryGetValue("language", out data))
            {
                d["Language"] = data[0];
            }
            else
            {
                d["Language"] = "C#";
            }

            if (node.Metadata.TryGetValue("autoeventwireup", out data))
            {
                d["AutoEventWireup"] = data[0];
            }
            else
            {
                d["AutoEventWireup"] = "true";
            }
            
            if (isPage) {
                d["Inherits"] = "System.Web.Mvc.ViewPage";
            } else {
                d["Inherits"] = "System.Web.Mvc.ViewUserControl";
            }

            if (node.Metadata.TryGetValue("inherits", out data))
            {
                d["Inherits"] = data[0];
            }

            if (node.Metadata.TryGetValue("type", out data))
            {
                d["Inherits"] = d["Inherits"] + "<" + data[1] + ">";
            }

            if (node.Metadata.TryGetValue("masterpagefile", out data))
            {
                d["MasterPageFile"] = data[0];
            }

            if (isPage)
            {
                WriteText("<%@ Page ");
            }
            else
            {
                WriteText("<%@ Control ");
            }
            foreach (var pair in d)
            {
                WriteText(pair.Key);
                WriteText("=\"");
                WriteText(pair.Value);
                WriteText("\" ");
            }

            WriteText(" %>" + System.Environment.NewLine);

            foreach(var child in node.Childs)
            {
                WriteText(System.Environment.NewLine);
                Visit(child);
            }
        }

        public string Result()
        {
            return _sb.ToString();           
        }

        protected override void WriteText(string text)
        {
            _sb.Append(text);
        }

        protected override void WriteCode(string code)
        {
            _sb.Append("<%= ");
            _sb.Append(code);
            _sb.Append(" %>");
        }

        protected override void PushWriter()
        {
            _stack.Push(_sb);
            _sb = new StringBuilder();
        }

        protected override string PopWriter()
        {
            var result = _sb.ToString();
            _sb = _stack.Pop();
            return result;
        }

        protected override void WriteStartBlock(string code, bool hasChild)
        {
            _sb.Append("<% " + code);
            if (hasChild)
            {
                _sb.Append(" { %>");
            }
            else
            {
                _sb.Append(" %>");
            }
        }

        protected override void WriteEndBlock()
        {
            _sb.Append("<% } %>");
        }
    }
}
