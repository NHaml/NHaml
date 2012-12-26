namespace System.Web.NHaml.Parser.Rules
{
    public class HamlNodeHtmlComment : HamlNode
    {
        public HamlNodeHtmlComment(IO.HamlLine nodeLine)
            : base(nodeLine)
        { }

        protected override bool IsContentGeneratingTag
        {
            get { return true; }
        }
    }
}
