using System;
using System.Collections.Generic;
using System.Text;

namespace NHaml.Core.Ast
{
    public class MetaNode : AstNode
    {
        public MetaNode(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            Name = name;
            Attributes = new List<AttributeNode>();
        }

        public string Name { get; set; }
        public string Value { get; set; }
        public List<AttributeNode> Attributes { get; private set; }
        public AstNode Child { get; set; }
    }
}
