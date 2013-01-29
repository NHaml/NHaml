namespace System.Web.NHaml.Parser.Exceptions
{
    [Serializable]
    public class HamlUnknownRuleException : Exception
    {
        public HamlUnknownRuleException(string ruleValue, int lineNo)
            : this(ruleValue, lineNo, null)
        { }

        private HamlUnknownRuleException(string ruleValue, int lineNo, Exception ex)
            : base(string.Format("Unknown rule '{0}' on line {1}", ruleValue, lineNo), ex)
        { }
    }
}
