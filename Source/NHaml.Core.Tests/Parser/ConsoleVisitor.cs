using System;
using NHaml.Core.Ast;

namespace NHaml.Core.Tests.Parser
{
    public class ConsoleVisitor : NodeVisitorBase
    {
        public void WriteIndent()
        {
            Console.Write(new String(' ', Indent * 2));
        }

        public override void Visit(TagNode node)
        {
            WriteIndent();
   
            Console.Write("<");
            Console.Write(node.Name);

            if(!string.IsNullOrEmpty(node.Id))
                Console.Write(" id=\"{0}\"", node.Id);

            if(!string.IsNullOrEmpty(node.Class))
                Console.Write(" class=\"{0}\"", node.Class);

            Console.WriteLine(">");

            base.Visit(node);

            WriteIndent();
            
            Console.Write("</");
            Console.Write(node.Name);
            Console.WriteLine(">");
        }

        public override void Visit(TextNode node)
        {
            WriteIndent();
            
            Console.WriteLine(node.Text);
        }
    }
}