using System;
using System.Collections.Generic;

namespace NHaml.Core.Ast
{
    public class TagNode : AstNode
    {
        public AstNode Child;

        public TagNode(string name)
        {
            if(name == null)
                throw new ArgumentNullException("name");

            Name = name;
            Attributes = new List<AttributeNode>();
        }

        public string Name { get; set; }

        public List<AttributeNode> Attributes { get; private set; }

        public AttributeNode GetOrAddAttribute(string name)
        {
            foreach(var attribute in Attributes)
            {
                if(attribute.Name == name)
                    return attribute;
            }

            var newAttribute = new AttributeNode(name);
            Attributes.Add(newAttribute);
            return newAttribute;
        }
    }
}