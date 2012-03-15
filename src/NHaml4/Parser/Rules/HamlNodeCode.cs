using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHaml4.IO;

namespace NHaml4.Parser.Rules
{
    public class HamlNodeCode : HamlNode
    {
        public HamlNodeCode(HamlLine line)
            : base(line) { }
    }
}
