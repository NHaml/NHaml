using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHaml4.Parser.Rules
{
    public class HamlNodeTextVariable : HamlNode
    {
        public HamlNodeTextVariable(IO.HamlLine nodeLine)
            : base(nodeLine)
        {
            
        }

        public bool IsWhitespace()
        {
            return Content.Trim().Length == 0;
        }
    }
}
