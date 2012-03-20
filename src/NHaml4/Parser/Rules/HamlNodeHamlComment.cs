using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHaml4.Parser.Rules
{
    public class HamlNodeHamlComment : HamlNode
    {
        public HamlNodeHamlComment(IO.HamlLine nodeLine)
            : base(nodeLine)
        { }

        public override bool IsContentGeneratingTag
        {
            get { return false; }
        }
    }
}
