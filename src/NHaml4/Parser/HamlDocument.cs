using NHaml4.IO;

namespace NHaml4.Parser
{
    public class HamlDocument : HamlNode
    {
        public HamlDocument(string fileName)
            : base(new HamlLine(fileName, HamlRuleEnum.Document))
        { }

        protected override bool IsContentGeneratingTag
        {
            get { return false; }
        }
    }
}
