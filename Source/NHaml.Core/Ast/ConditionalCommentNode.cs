namespace NHaml.Core.Ast
{
    public class ConditionalCommentNode : AstNode
    {
        public string Condition { get; set; }

        public AstNode Child { get; set; }
    }
}