namespace NHaml.Core.Ast
{
    public abstract class AstNode
    {
        public SourceInfo StartInfo { get; set; }
        public SourceInfo EndInfo { get; set; }
    }
}