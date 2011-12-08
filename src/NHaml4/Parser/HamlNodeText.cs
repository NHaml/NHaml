using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHaml4.Parser
{
    public class HamlNodeText : HamlNode
    {
        private IO.HamlLine _nodeLine;

        public HamlNodeText(IO.HamlLine nodeLine)
        {
            _nodeLine = nodeLine;
        }

        public string Text
        {
            get { return _nodeLine.Content; }
        }
    }
}
