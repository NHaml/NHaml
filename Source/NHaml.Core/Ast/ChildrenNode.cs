using System;
using System.Collections;
using System.Collections.Generic;

namespace NHaml.Core.Ast
{
    public class ChildrenNode : AstNode, IEnumerable<AstNode>
    {
        private readonly IEnumerable<AstNode> _children;

        public ChildrenNode(IEnumerable<AstNode> children)
        {
            if(children == null)
                throw new ArgumentNullException("children");

            _children = children;
        }

        public IEnumerator<AstNode> GetEnumerator()
        {
            return _children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count
        {
            get
            {
                var counter = 0;
                
                foreach(var node in _children)
                    counter++;
                
                return counter;
            }
        }
    }
}