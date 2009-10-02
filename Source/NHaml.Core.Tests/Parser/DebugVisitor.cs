using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using NHaml.Core.Ast;

namespace NHaml.Core.Tests.Parser
{
    public class DebugVisitor : NodeVisitorBase
    {
        private TextWriter _writer;

        public Dictionary<string,string> Locals = new Dictionary<string, string>();

        public DebugVisitor(TextWriter writer)
        {
            if(writer == null)
                throw new ArgumentNullException("writer");

            _writer = writer;
        }

        public void WriteIndent()
        {
            _writer.Write(new String(' ', Indent*2));
        }

        public override void Visit(DocTypeNode node)
        {
            if(string.IsNullOrEmpty(node.Text))
            {
                _writer.Write(
                    @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">");
                return;
            }

            switch(node.Text)
            {
                case "1.1":
                {
                    _writer.Write(@"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.1//EN"" ""http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd"">");
                    break;
                }
                case "frameset":
                {
                    _writer.Write(
                        @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Frameset//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-frameset.dtd"">");
                    break;
                }
                case "basic":
                {
                    _writer.Write(@"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML Basic 1.1//EN"" ""http://www.w3.org/TR/xhtml-basic/xhtml-basic11.dtd"">");
                    break;
                }
                case "XML":
                {
                    _writer.Write(@"<?xml version='1.0' encoding='utf-8' ?>");
                    break;
                }
                case "strict":
                {
                    _writer.Write(@"<!DOCTYPE html PUBLIC ""-//W3C//DTD HTML 4.01//EN"" ""http://www.w3.org/TR/html4/strict.dtd"">");
                    break;
                }
                case "mobile":
                {
                    _writer.Write(
                        @"<!DOCTYPE html PUBLIC ""-//WAPFORUM//DTD XHTML Mobile 1.2//EN"" ""http://www.openmobilealliance.org/tech/DTD/xhtml-mobile12.dtd"">");
                    break;
                }
                default:
                    throw new Exception("unknown doctype");
            }
        }

        public override void Visit(ChildrenNode node)
        {
            VisitAndIdentAllways(node);
        }

        private void VisitAndIdentAllways(AstNode node)
        {
            if(node == null)
                return;

            _writer.WriteLine();

            if(node is ChildrenNode)
                foreach(var childrenNode in (ChildrenNode)node)
                {
                    WriteIndent();
                    Visit(childrenNode);
                    _writer.WriteLine();
                }
            else
            {
                WriteIndent();
                Visit(node);
                _writer.WriteLine();
            }
        }

        private void VisitAndIdentOnlyWithMoreChilds(AstNode node)
        {
            if(node == null)
                return;

            if(node is ChildrenNode)
            {
                var children = ( (ChildrenNode)node );
                if(children.Count > 1)
                    VisitAndIdentAllways(node);
                else
                    foreach(var child in children)
                        node = child;
            }

            Visit(node);
        }

        public override void Visit(TagNode node)
        {
            _writer.Write("<{0}", node.Name);
            
            var queue = new List<AttributeNode>(node.Attributes);
            
            var attributes = new List<AttributeNode>();
            while(queue.Count>0)
            {
                var first = queue[0];
                queue.RemoveAt(0);
                attributes.Add(first);

                var isId = first.Name.Equals("id", StringComparison.InvariantCultureIgnoreCase);
                
                var buf = new List<string> {Capture(first.Value)};

                foreach(var sameAtt in queue.FindAll(a => a.Name == first.Name))
                {
                    queue.Remove(sameAtt);
                    buf.Add(Capture(sameAtt.Value));
                }

                if(!isId)
                    buf.Sort((a1, a2) => a1.CompareTo(a2));

                first.Value = new TextNode(string.Join(isId?"_":" ", buf.ToArray()));
            }

            attributes.Sort((a1, a2) => a1.Name.CompareTo(a2.Name));

            foreach(var attribute in attributes)
            {
                _writer.Write(' ');
                Visit(attribute);
            }
            
            if(node.Child == null && node.Name.Equals("meta", StringComparison.InvariantCultureIgnoreCase))
            {
                _writer.Write(" />");
                return;
            }

            _writer.Write(">");

            Visit(node.Child);

            _writer.Write("</{0}>", node.Name);
        }

        public override void Visit(TextNode node)
        {
            _writer.Write(node.Text);
        }

        public override void Visit(AttributeNode node)
        {
            _writer.Write(node.Name);
            _writer.Write("='");

            base.Visit(node);

            _writer.Write("'");
        }

        public override void Visit(FilterNode node)
        {
            switch(node.Name)
            {
                case "javascript":
                {
                    _writer.WriteLine(@"<script type='text/javascript'>");
                    _writer.Write(@"  //<![CDATA[");

                    Indent++;
                    VisitAndIdentAllways(node.Child);
                    Indent--;
                    
                    _writer.WriteLine(@"  //]]>");
                    _writer.WriteLine(@"</script>");
                    break;
                }
                case "preserve":
                {
                    var replace = Capture(() => VisitAndIdentOnlyWithMoreChilds(node.Child));

                    if(!replace.Contains(_writer.NewLine))
                        replace += _writer.NewLine;

                    _writer.WriteLine(replace.Replace(_writer.NewLine, "&#x000A;"));

                    break;
                }
                case "plain":
                {
                    _writer.WriteLine(Capture(() => VisitAndIdentOnlyWithMoreChilds(node.Child)));
                    break;
                }

                case "escaped":
                {
                    var text = Capture(() => VisitAndIdentOnlyWithMoreChilds(node.Child));
                    _writer.Write(HttpUtility.HtmlEncode(text));
                    break;
                }
                default:
                    throw new Exception("filter not found");
            }
        }

        public override void Visit(ConditionalCommentNode node)
        {
            _writer.Write("<!--[{0}]>", node.Condition);

            VisitAndIdentAllways(node.Child);

            _writer.Write("<![endif]-->");
        }

        public override void Visit(CommentNode node)
        {
            _writer.Write("<!--");

            var space = !( node.Child is ChildrenNode );

            if(space)
                _writer.Write(' ');

            base.Visit(node);

            if(space)
                _writer.Write(' ');

            _writer.Write("-->");
        }

        public override void Visit(LocalNode node)
        {
            string value;
            if(Locals.TryGetValue(node.Name, out value))
                _writer.Write(value);
            else
                _writer.Write("~notfound~");
        }

        public string Capture(AstNode node)
        {
            if(node == null)
                throw new ArgumentNullException("node");

            return Capture(() => Visit(node));
        }

        public string Capture(Action action)
        {
            if(action == null)
                throw new ArgumentNullException("action");

            var savedWriter = _writer;
            var outputWriter = new StringWriter();
            _writer = outputWriter;

            action();

            _writer = savedWriter;

            return outputWriter.ToString();
        }
    }
}