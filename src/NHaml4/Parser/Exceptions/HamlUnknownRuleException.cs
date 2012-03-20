using System;

namespace NHaml4.Parser.Exceptions
{
    [Serializable]
    public class HamlUnknownRuleException : Exception
    {
        public HamlUnknownRuleException(string ruleValue, int lineNo)
            : this(ruleValue, lineNo, null)
        { }

        public HamlUnknownRuleException(string ruleValue, int lineNo, Exception ex)
            : base(string.Format("Unknown rule '{0}' on line {1}", ruleValue, lineNo), ex)
        { }
    }
}
