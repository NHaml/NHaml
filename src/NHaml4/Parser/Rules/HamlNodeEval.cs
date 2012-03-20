using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHaml4.IO;

namespace NHaml4.Parser.Rules
{
    public class HamlNodeEval : HamlNode
    {
        public HamlNodeEval(HamlLine line)
            : base(line) { }

        public override bool IsContentGeneratingTag
        {
            get { return true; }
        }
    }
}
