using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHaml4.Parser.Rules
{
    public class HamlNodeTagId : HamlNode
    {
        public HamlNodeTagId(string tagId)
        {
            Content = tagId;
        }
    }
}
