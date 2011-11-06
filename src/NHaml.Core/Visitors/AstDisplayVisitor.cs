using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using NHaml.Core.Ast;
using NHaml.Core.Template;

namespace NHaml.Core.Visitors
{
    /// <summary>
    /// Abstract class that simply shows the AST
    /// </summary>
    public class AstDisplayVisitor : NodeVisitorBase
    {
        public AstDisplayVisitor(TextWriter output)
        {
            _output = output;
        }

        private TextWriter _output;
        public int Indent { get; set; }

        public override void Visit(DocumentNode node)
        {
            WriteIndent();
            WriteText("DocumentNode");
            WriteText(System.Environment.NewLine);
            foreach (var child in node.Childs)
            {
                Indent++;
                Visit(child);
                Indent--;
            }
        }

        public override void Visit(DocTypeNode node)
        {
            WriteIndent();
            WriteText("DocTypeNode ");
            WriteText(node.Text);
            WriteText(System.Environment.NewLine);
        }

        public override void Visit(ChildrenNode node)
        {
            WriteIndent();
            WriteText("ChildrenNode");
            WriteText(System.Environment.NewLine);
            Indent++;
            VisitAndIdentAlways(node);
            Indent--;
        }

        public override void Visit(TagNode node)
        {
            WriteIndent();
            WriteText("TagNode ");
            WriteText(node.Name);
            WriteText(System.Environment.NewLine);

            Indent++;
            WriteIndent();
            WriteText("Attributes");
            WriteText(System.Environment.NewLine);
            Indent++;
            foreach (var attribute in node.Attributes)
            {
                Visit(attribute);
            }
            Indent--;
            WriteIndent();
            WriteText("Childs");
            WriteText(System.Environment.NewLine);
            Indent++;
            Visit(node.Child);
            Indent--;
            Indent--;
        }

        public override void Visit(TextNode node)
        {
            WriteIndent();
            WriteText("TextNode");
            WriteText(System.Environment.NewLine);
            Indent++;
            foreach (var chunk in node.Chunks)
                Visit(chunk);
            Indent--;
        }

        public override void Visit(TextChunk chunk)
        {
            WriteIndent();
            WriteText("TextChunk ");
            WriteText(chunk.Text);
            WriteText(System.Environment.NewLine);
        }

        public override void Visit(CodeChunk chunk)
        {
            WriteIndent();
            WriteText("CodeChunk ");
            WriteText(chunk.Code);
            WriteText(" ");
            WriteText(chunk.Escape.ToString());
            WriteText(System.Environment.NewLine);
        }

        public override void Visit(AttributeNode node)
        {
            WriteIndent();
            WriteText("AttributeNode");
            WriteText(System.Environment.NewLine);
            Indent++;
            base.Visit(node);
            Indent--;
        }

        public override void Visit(CodeBlockNode node)
        {
            WriteIndent();
            WriteText("CodeBlockNode ");
            WriteText(node.Code);
            WriteText(System.Environment.NewLine);
            if (node.Child != null)
            {
                Indent++;
                Visit(node.Child);
                Indent--;
            }
        }

        public override void Visit(FilterNode node)
        {
            WriteIndent();
            WriteText("FilterNode ");
            WriteText(node.Name);
            WriteText(System.Environment.NewLine);
            Indent++;
            if (node.Child != null)
                Visit(node.Child);
            Indent--;
        }

        public override void Visit(CommentNode node)
        {
            WriteIndent();
            WriteText("CommentNode ");
            WriteText(node.Condition);
            WriteText(System.Environment.NewLine);
            Indent++;
            if (node.Child != null)
                Visit(node.Child);
            Indent--;
        }

        public override void Visit(CodeNode node)
        {
            WriteIndent();
            WriteText("CodeNode ");
            WriteText(node.Code);
            WriteText(" ");
            WriteText(node.Escape.ToString());
            WriteText(System.Environment.NewLine);
        }

        private void VisitAndIdentAlways(AstNode node)
        {
            if (node == null)
                return;

            if (node is ChildrenNode)
            {
                foreach (var childrenNode in (ChildrenNode)node)
                {
                    Visit(childrenNode);
                }
            }
            else
            {
                Visit(node);
            }
        }

        public virtual void WriteIndent()
        {
            WriteText(new String(' ',Indent*2));
        }

        protected void WriteText(string text)
        {
            _output.Write(text);
        }
    }
}