using NHaml4.IO;

namespace NHaml4.Parser.Rules
{
    public class HamlNodeCode : HamlNode
    {
        public HamlNodeCode(HamlLine line)
            : base(line) { }

        protected override bool IsContentGeneratingTag
        {
            get { return false; }
        }
    }
}
