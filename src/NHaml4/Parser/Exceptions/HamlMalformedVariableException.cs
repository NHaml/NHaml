using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHaml4.Parser.Exceptions
{
    [Serializable]
    public class HamlMalformedVariableException : Exception
    {
        public HamlMalformedVariableException(string variable, int lineNo)
            : base(string.Format("Malformed variable on line {0} : {1}", lineNo, variable))
        { }
    }
}
