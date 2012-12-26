namespace System.Web.NHaml.Parser.Rules
{
    public class HamlNodeTagClass : HamlNode
    {
        public HamlNodeTagClass(int sourceFileLineNo, string className)
            : base(sourceFileLineNo, className)
        { }

        protected override bool IsContentGeneratingTag
        {
            get { return true; }
        }
    }
}
