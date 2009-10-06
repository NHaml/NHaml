using System;
using System.Diagnostics;

namespace NHaml.Core.Ast
{
    public abstract class NodeVisitorBase
    {
        public virtual void Visit(DocumentNode node)
        {
            foreach(var chield in node.Childs)
                Visit(chield);
        }

        public virtual void Visit(TagNode node)
        {
            if(node.Child != null)
                Visit(node.Child);
        }

        public virtual void Visit(TextNode node)
        {
        }

        public virtual void Visit(FilterNode node)
        {
            if(node.Child != null)
                Visit(node.Child);
        }

        public virtual void Visit(ChildrenNode node)
        {
            foreach(var children in node)
                Visit(children);
        }

        public virtual void Visit(CommentNode node)
        {
            if(node.Child != null)
                Visit(node.Child);
        }

        public virtual void Visit(AttributeNode node)
        {
            if(node.Value != null)
                Visit(node.Value);
        }

        public virtual void Visit(DocTypeNode node)
        {
        }

        public virtual void Visit(CodeNode node)
        {
        }

        public virtual void Visit(TextChunk chunk)
        {
        }

        public virtual void Visit(CodeChunk chunk)
        {
        }

        [DebuggerStepThrough]
        public virtual void Visit(AstNode node)
        {
            if(node == null)
                return;

            if(node is ChildrenNode)
                Visit((ChildrenNode)node);
            else if(node is TextNode)
                Visit((TextNode)node);
            else if(node is TagNode)
                Visit((TagNode)node);
            else if(node is FilterNode)
                Visit((FilterNode)node);
            else if(node is CommentNode)
                Visit((CommentNode)node);
            else if(node is AttributeNode)
                Visit((AttributeNode)node);
            else if(node is DocTypeNode)
                Visit((DocTypeNode)node);
            else if(node is CodeNode)
                Visit((CodeNode)node);
            else if(node is TextChunk)
                Visit((TextChunk)node);
            else if(node is CodeChunk)
                Visit((CodeChunk)node);
            else
                throw new InvalidOperationException("unknown node");
        }
    }
}