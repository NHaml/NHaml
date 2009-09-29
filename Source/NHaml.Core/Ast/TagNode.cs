using System;
using System.Collections.Generic;

namespace NHaml.Core.Ast
{
    public class TagNode : AstNode
    {
        public List<AstNode> Chields = new List<AstNode>();

        public TagNode(string name)
        {
            if(name == null)
                throw new ArgumentNullException("name");

            Name = name;
        }

        public string Name { get; set; }

        public string Class { get; set; }

        public string Id { get; set; }
    }
}