using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHaml4.Parser.Rules
{
    public class HamlNodeHtmlComment : HamlNode
    {
        public HamlNodeHtmlComment(IO.HamlLine nodeLine)
            : base(nodeLine)
        { }

        public override bool IsContentGeneratingTag
        {
            get { return true; }
        }
    }
}
