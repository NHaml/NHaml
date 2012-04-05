using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHaml4.IO;

namespace NHaml4.Parser.Rules
{
    public class HamlNodeDocType : HamlNode
    {
        public HamlNodeDocType(HamlLine line)
            : base(line) { }

        public override bool IsContentGeneratingTag
        {
            get { return true; }
        }
    }
}
