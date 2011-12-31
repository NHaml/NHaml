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
        private readonly string _content;
        private readonly string _indent;
        private int _indentCount;
        private IList<HamlNode> _children = new List<HamlNode>();
        private bool _multiLine;

        public HamlNode()
        {
        }

        public HamlNode(HamlLine nodeLine)
        {
            _content = nodeLine.Content;
            _indent = nodeLine.Indent;
            _indentCount = nodeLine.IndentCount;
        }

        public string Content
        {
            get { return _content; }
        }

        public string Indent
        {
            get { return _indent; }
        }

        public bool IsMultiLine {
            get { return _multiLine; }
            set {_multiLine = value; }
        }

        public int IndentCount
        {
            get { return _indentCount; }
            protected set { _indentCount = value; }
        }

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
