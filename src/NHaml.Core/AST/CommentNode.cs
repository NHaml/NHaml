namespace NHaml.Core.Ast
{
    public class CommentNode : AstNode
    {
        public string Condition { get; set; }

        public AstNode Child { get; set; }
    }
}