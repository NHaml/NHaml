using System;

namespace NHaml.Core.Ast
{
    public class AttributeNode : AstNode
    {
        public AttributeNode(string name)
        {
            if(name == null)
                throw new ArgumentNullException("name");

            Name = name;
        }

        public string Name { get; set; }
        public AstNode Value { get; set; }
    }
}