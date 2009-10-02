using System;

namespace NHaml.Core.Ast
{
    public class LocalNode : AstNode
    {
        public LocalNode(string name)
        {
            if(name == null)
                throw new ArgumentNullException("name");

            Name = name;
        }

        public string Name { get; set; }
    }
}