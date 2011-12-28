using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHaml4.IO;
using System.Collections.ObjectModel;

namespace NHaml4.Parser
{
    public abstract class HamlNode : IEnumerable<HamlNode>
    {
        private IList<HamlNode> _children = new List<HamlNode>();

        public ReadOnlyCollection<HamlNode> Children
        {
            get { return new ReadOnlyCollection<HamlNode>(_children); }
        }

        public void Add(HamlNode hamlNode)
        {
            _children.Add(hamlNode);
        }


        public IEnumerator<HamlNode> GetEnumerator()
        {
            return _children.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _children.GetEnumerator();
        }
    }
}
