using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHaml4.Walkers.CodeDom
{
    class HamlNodeWalkerException : Exception
    {
        public HamlNodeWalkerException(string hamlNodeType, int lineNo, Exception e)
            : base(
                string.Format("Exception occurred walking {0} HamlNode on line {1}.", hamlNodeType, lineNo),
                e)
        { }
    }
}
