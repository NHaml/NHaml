using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHaml4.Parser
{
    class HamlUnknownRuleException : Exception
    {
        private string p;

        public HamlUnknownRuleException(string ruleValue)
            : this(ruleValue, null)
        { }

        public HamlUnknownRuleException(string ruleValue, Exception ex)
            : base(string.Format("Unable to create node for unknown rule {0}", ruleValue), ex)
        { }
    }
}
