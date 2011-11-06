using System;

namespace NHaml.Core.Ast
{
    public class FilterNode : AstNode
    {
        public FilterNode(string name)
        {
            if(name == null)
                throw new ArgumentNullException("name");

            Name = name;
        }

        public string Name { get; set; }
        public AstNode Child { get; set; }
    }
}