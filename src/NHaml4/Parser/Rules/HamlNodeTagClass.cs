using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHaml4.Parser.Rules
{
    public class HamlNodeTagClass : HamlNode
    {
        public HamlNodeTagClass(string className)
        {
            Content = className;
        }
    }
}
