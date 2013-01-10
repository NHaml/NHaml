using System.Web.NHaml.IO;

namespace System.Web.NHaml.Parser.Rules
{
    public class HamlNodeEval : HamlNode
    {
        public HamlNodeEval(HamlLine line)
            : base(line) { }

        protected override bool IsContentGeneratingTag
        {
            get { return true; }
        }
    }
}
