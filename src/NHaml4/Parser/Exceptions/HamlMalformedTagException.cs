using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHaml4.Parser.Exceptions
{
    [Serializable]
    public class HamlMalformedTagException : Exception
    {
        public HamlMalformedTagException(string message, int lineNo)
            : base(string.Format("Malformed tag on line {0} : {1}", lineNo, message))
        { }
    }
}
