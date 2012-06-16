namespace NHaml4.Parser
{
    public class HamlDocument : HamlNode
    {
        public HamlDocument(string fileName)
            : base(0, fileName)
        { }

        protected override bool IsContentGeneratingTag
        {
            get { return false; }
        }
    }
}
