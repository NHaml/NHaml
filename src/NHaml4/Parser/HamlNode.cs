using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHaml4.IO;

namespace NHaml.Parser
{
    public class HamlNode
    {
        private IList<HamlNode> _children = new List<HamlNode>();
        private HamlLine currentLine;

        public HamlNode(HamlLine currentLine)
        {
            // TODO: Complete member initialization
            this.currentLine = currentLine;
        }

        public IList<HamlNode> Children
        {
            get
            {
                return _children;
            }
        }

        internal void AddChild(HamlNode hamlNode)
        {
            _children.Add(hamlNode);
        }
    }
}
