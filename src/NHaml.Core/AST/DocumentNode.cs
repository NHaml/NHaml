using System.Collections.Generic;

namespace NHaml.Core.Ast
{
    public class DocumentNode : AstNode
    {
        public List<AstNode> Childs = new List<AstNode>();
        public Dictionary<string, List<string>> Metadata = new Dictionary<string, List<string>>();
    }
}