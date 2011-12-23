using System;

namespace NHaml4.Parser
{
    [Serializable]
    public class HamlUnknownRuleException : Exception
    {
        public HamlUnknownRuleException(string ruleValue)
            : this(ruleValue, null)
        { }

        public HamlUnknownRuleException(string ruleValue, Exception ex)
            : base(string.Format("Unable to create node for unknown rule {0}", ruleValue), ex)
        { }
    }
}
