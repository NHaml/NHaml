using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHaml4.Parser
{
    public class HamlNodeText : HamlNode
    {
        private readonly string _text;

        public HamlNodeText(IO.HamlLine nodeLine)
            : this(nodeLine.Content)
        { }

        public HamlNodeText(string nodeText)
        {
            _text = nodeText;
        }

        public string Text
        {
            get { return _text; }
        }
    }
}
