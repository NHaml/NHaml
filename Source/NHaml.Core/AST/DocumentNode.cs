using System.Collections.Generic;

namespace NHaml.Core.Ast
{
    public class DocumentNode : AstNode
    {
        public List<AstNode> Chields = new List<AstNode>();
    }
}