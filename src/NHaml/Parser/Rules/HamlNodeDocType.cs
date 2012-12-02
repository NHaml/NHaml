using NHaml.IO;

namespace NHaml.Parser.Rules
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
