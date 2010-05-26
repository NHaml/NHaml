using System;
using System.Collections.Generic;
using NHaml.Core.IO;

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
            AutoClose = false;
        }

        public string Name { get; set; }

        public List<AttributeNode> Attributes { get; private set; }
        public bool AutoClose { get; set; }

        public SourceInfo OperatorInfo{get;set;}
    }
}