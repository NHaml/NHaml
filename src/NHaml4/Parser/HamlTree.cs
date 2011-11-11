using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHaml4.IO;

namespace NHaml.Parser
{
    public class HamlTree
    {
        private IList<HamlLine> _children = new List<HamlLine>();

        public IList<HamlLine> Children {
            get
            {
                return _children;
            }
        }

        internal void AddChild(HamlLine hamlLine)
        {
            _children.Add(hamlLine);
        }
    }
}
