namespace NHaml4.Parser.Rules
{
    public class HamlNodeTextVariable : HamlNode
    {
        public HamlNodeTextVariable(string content, int sourceLineNum)
            : base(sourceLineNum, content)
        { }

        protected override bool IsContentGeneratingTag
        {
            get { return true; }
        }
    }
}
