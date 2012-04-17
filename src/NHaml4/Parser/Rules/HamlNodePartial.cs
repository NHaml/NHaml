using NHaml4.IO;

namespace NHaml4.Parser.Rules
{
    public class HamlNodePartial : HamlNode
    {
        public HamlNodePartial(HamlLine line) : base(line) {
            IsResolved = false;
        }

        public override bool IsContentGeneratingTag
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
