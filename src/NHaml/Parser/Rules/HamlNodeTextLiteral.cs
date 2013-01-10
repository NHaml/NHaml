namespace System.Web.NHaml.Parser.Rules
{
    public class HamlNodeTextLiteral : HamlNode
    {
        public HamlNodeTextLiteral(int sourceLineNum, string content)
            : base(sourceLineNum, content)
        { }

        protected override bool IsContentGeneratingTag
        {
            get { return true; }
        }
    }
}
