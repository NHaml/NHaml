using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHaml4.IO;
using System.Collections.ObjectModel;

namespace NHaml4.Parser
{
    public abstract class HamlNode
    {
        private string _content;
        private IList<HamlNode> _children = new List<HamlNode>();
        private bool _multiLine;
        private HamlLine _line;
        private int _sourceFileLineNo;

        //public HamlLine HamlLine
        //{
        //    get { return _line; }
        //}

        public HamlNode(HamlLine nodeLine)
        {
            _line = nodeLine;
            _content = nodeLine.Content;
            _sourceFileLineNo = _line.SourceFileLineNo;
        }

        public HamlNode(int sourceFileLineNo, string content)
        {
            _sourceFileLineNo = sourceFileLineNo;
            _content = content;
        }

        public string Content
        {
            get { return _content; }
            protected set { _content = value; }
        }

        public string Indent
        {
            get
            {
                return _line == null
                    ? ""
                    : _line.Indent;
            }
        }

        public bool IsMultiLine {
            get { return _multiLine; }
            set {_multiLine = value; }
        }

        public int IndentCount
        {
            get
            {
                return _line == null
                    ? -1
                    : _line.IndentCount;
            }
        }

        public int SourceFileLineNo
        {
            protected set
            {
                _sourceFileLineNo = value;
            }
            get
            {
                return _sourceFileLineNo;
            }
        }

        public ReadOnlyCollection<HamlNode> Children
        {
            get { return new ReadOnlyCollection<HamlNode>(_children); }
        }

        public void AddChild(HamlNode hamlNode)
        {
            hamlNode.Parent = this;
            hamlNode.Previous = _children.LastOrDefault();
            if (hamlNode.Previous != null)
            {
                hamlNode.Previous.Next = hamlNode;
            }

            _children.Add(hamlNode);
        }

        public HamlNode Previous { get; set; }
        public HamlNode Next { get; set; }
        public HamlNode Parent { get; set; }
    }
}
