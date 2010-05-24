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

        public override void Visit(MetaNode node)
        {
            if (node.Name == "contentplaceholder")
            {
                bool hasChild = node.Child != null;
                WriteText(String.Format("<asp:ContentPlaceHolder ID=\"{0}\" runat=\"server\"", node.Value));
                if (hasChild)
                {
                    WriteText(">");
                    WriteText(Environment.NewLine);
                    Indent++;
                    Visit(node.Child);
                    Indent--;
                    WriteText("</asp:ContentPlaceHolder>");
                }
                else
                {
                    WriteText(" />");
                }
            }
            else if (node.Name == "partialcontent")
            {
                string obj = null;
                foreach (var attr in node.Attributes)
                {
                    if (attr.Name == "model")
                    {
                        obj = ((TextChunk)(((TextNode)attr.Value).Chunks[0])).Text;
                    }
                }
                if (obj != null)
                {
                    WriteCode(String.Format("Html.RenderPartial(\"{0}\")",node.Value),false);
                }
                else
                {
                    WriteCode(String.Format("Html.RenderPartial(\"{0}\",{1})", node.Value, obj), false);
                }
            }
            else if (node.Name == "content")
            {
                WriteText(String.Format("<asp:Content ID=\"{0}\" runat=\"server\">", node.Value));
                WriteText(Environment.NewLine);
                Indent++;
                if (node.Child!=null)
                    Visit(node.Child);
                Indent--;
                WriteText(Environment.NewLine);
                WriteIndent();
                WriteText("</asp:Content>");
            }
            else
            {
                base.Visit(node);
            }
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
            if (node.Metadata.TryGetValue("control",out data))
            {
                pagedefiniton = data[0];
            }
            if (node.Metadata.TryGetValue("master", out data))
            {
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
                else if (pagedefiniton.Name == "control")
                    pagedefiniton.Attributes.Add(new AttributeNode("Inherits") { Value = new TextNode(new TextChunk("System.Web.Mvc.ViewUserControl")) });
                else
                    pagedefiniton.Attributes.Add(new AttributeNode("Inherits") { Value = new TextNode(new TextChunk("System.Web.Mvc.ViewMasterPage")) });
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
            else if (pagedefiniton.Name == "control")
            {
                WriteText("<%@ Control ");
            }
            else
            {
                WriteText("<%@ Master ");
            }
            foreach (var attr in pagedefiniton.Attributes)
            {
                WriteText(attr.Name);
                WriteText("=\"");
                WriteText(((attr.Value as TextNode).Chunks[0] as TextChunk).Text);
                WriteText("\" ");
            }
            WriteText(" %>" + System.Environment.NewLine);

            bool hasContentMetaFlags = node.Metadata.ContainsKey("content");

            if (!hasContentMetaFlags)
            {
                WriteText("<asp:Content ID=\"Main\" runat=\"server\">");
                WriteText(Environment.NewLine);
                Indent++;
            }

            foreach(var child in node.Childs)
            {
                WriteText(System.Environment.NewLine);
                Visit(child);
            }

            if (!hasContentMetaFlags)
            {
                WriteText(System.Environment.NewLine);
                Indent--;
                WriteIndent();
                WriteText("</asp:Content>");
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
