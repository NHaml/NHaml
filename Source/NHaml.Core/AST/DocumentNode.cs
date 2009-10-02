using System.Collections.Generic;

namespace NHaml.Core.Ast
{
    public class DocumentNode : AstNode
    {
        public List<AstNode> Childs = new List<AstNode>();
    }
}