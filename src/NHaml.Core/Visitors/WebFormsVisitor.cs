using System;
using System.Collections.Generic;
using System.Text;
using NHaml.Core.Ast;
using System.Web;

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
            List<MetaNode> data;
            if (node.Metadata.TryGetValue("namespace",out data)) {
                foreach (MetaNode str in data) {
                    WriteText("<%@ Import Namespace=\""+str.Value+"\" %>" + Environment.NewLine);
                }
            }

            if (node.Metadata.TryGetValue("assembly",out data)) {
                foreach (MetaNode str in data) {
                    WriteText("<%@ Assembly Name=\"" + str.Value + "\" %>" + Environment.NewLine);
                }
            }

            var pagedefiniton = new MetaNode("page");

            if (node.Metadata.TryGetValue("page", out data))
            {
                pagedefiniton = data[0];
            }
            if (node.Metadata.TryGetValue("control",out data)) {
                pagedefiniton = data[0];
            }

            if (pagedefiniton.Attributes.Find(x => x.Name == "Language") == null)
            {
                pagedefiniton.Attributes.Add(new AttributeNode("Language") { Value = new TextNode(new TextChunk("C#")) });
            }

            if (pagedefiniton.Attributes.Find(x => x.Name == "AutoEventWireup") == null)
            {
                pagedefiniton.Attributes.Add(new AttributeNode("AutoEventWireup") { Value = new TextNode(new TextChunk("true")) });
            }

            if (pagedefiniton.Attributes.Find(x => x.Name == "Inherits") == null)
            {
                if (pagedefiniton.Name == "page")
                    pagedefiniton.Attributes.Add(new AttributeNode("Inherits") { Value = new TextNode(new TextChunk("System.Web.Mvc.ViewPage")) });
                else
                    pagedefiniton.Attributes.Add(new AttributeNode("Inherits") { Value = new TextNode(new TextChunk("System.Web.Mvc.ViewUserControl")) });
            }           

            if (node.Metadata.TryGetValue("type", out data))
            {
                var tc = pagedefiniton.Attributes.Find(x => x.Name == "Inherits");
                tc.Value = new TextNode(new TextChunk(((tc.Value as TextNode).Chunks[0] as TextChunk).Text + "<" + data[0].Value + ">"));
            }

            if (pagedefiniton.Attributes.Find(x => x.Name == "MasterPageFile") == null)
            {
                pagedefiniton.Attributes.Add(new AttributeNode("MasterPageFile") { Value = new TextNode(new TextChunk("true")) });
            }

            if (pagedefiniton.Name == "page")
            {
                WriteText("<%@ Page ");
            }
            else
            {
                WriteText("<%@ Control ");
            }
            foreach (var attr in pagedefiniton.Attributes)
            {
                WriteText(attr.Name);
                WriteText("=\"");
                WriteText(((attr.Value as TextNode).Chunks[0] as TextChunk).Text);
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

        protected override void WriteCode(string code, bool escapeHtml)
        {
            _sb.Append("<%= ");
            if (escapeHtml) _sb.Append("Html.Encode(");
            _sb.Append(code);
            if (escapeHtml) _sb.Append(")");
            _sb.Append(" %>");
        }

        protected override void PushWriter()
        {
            _stack.Push(_sb);
            _sb = new StringBuilder();
        }

        protected override object PopWriter()
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

        protected override void WriteData(object data, string filter)
        {
            if (filter == null)
            {
                WriteText(data as string);
                return;
            }
            switch (filter)
            {
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
            foreach (object o in data) {
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
