using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHaml4.Parser.Rules
{
    public class HamlNodeTagId : HamlNode
    {
        public HamlNodeTagId(int sourceFileLineNo, string tagId)
            : base(sourceFileLineNo, tagId)
        { }

        public override bool IsContentGeneratingTag
        {
            get { return true; }
        }
    }
}
