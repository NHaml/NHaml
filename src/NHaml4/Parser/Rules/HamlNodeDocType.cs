using NHaml4.IO;

namespace NHaml4.Parser.Rules
{
    public class HamlNodeDocType : HamlNode
    {
        public HamlNodeDocType(HamlLine line)
            : base(line) { }

        protected override bool IsContentGeneratingTag
        {
            get { return true; }
        }
    }
}
