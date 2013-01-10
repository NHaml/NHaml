using System.Web.NHaml.IO;

namespace System.Web.NHaml.Parser.Rules
{
    public class HamlNodePartial : HamlNode
    {
        public HamlNodePartial(HamlLine line) : base(line) {
            IsResolved = false;
        }

        protected override bool IsContentGeneratingTag
        {
            get { return false; }
        }

        public void SetDocument(HamlDocument partialDocument)
        {
            foreach (var child in partialDocument.Children)
                AddChild(child);
            IsResolved = true;
        }

        public bool IsResolved { get; private set; }
    }
}
