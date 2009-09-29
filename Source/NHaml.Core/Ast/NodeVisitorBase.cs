using System;

namespace NHaml.Core.Ast
{
    public abstract class NodeVisitorBase
    {
        public int Indent { get; private set; }

        public virtual void Visit(DocumentNode node)
        {
            foreach(var chield in node.Chields)
                Visit(chield);
        }

        public virtual void Visit(TagNode node)
        {
            Indent++;
            
            foreach(var chield in node.Chields)
                Visit(chield);
            
            Indent--;
        }

        public virtual void Visit(TextNode node)
        {
        }

        public virtual void Visit(AstNode node)
        {
            if(node is TextNode)
                Visit((TextNode)node);
            else if(node is TagNode)
                Visit((TagNode)node);
            else
                throw new InvalidOperationException("unknown node");
        }
    }
}