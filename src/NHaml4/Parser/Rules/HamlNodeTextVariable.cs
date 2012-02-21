using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHaml4.Parser.Rules
{
    public class HamlNodeTextVariable : HamlNode
    {
        public HamlNodeTextVariable(string content, int sourceLineNum)
            : base(sourceLineNum, content)
        {
            
        }

        public bool IsWhitespace()
        {
            return Content.Trim().Length == 0;
        }
    }
}
