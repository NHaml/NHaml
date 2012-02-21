using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHaml4.IO;
using System.Collections.ObjectModel;
using NHaml4.Parser.Rules;

namespace NHaml4.Parser
{
    public abstract class HamlNode
    {
        private string _content;
        private readonly IList<HamlNode> _children = new List<HamlNode>();
        private bool _multiLine;
        private readonly HamlLine _line;
        private int _sourceFileLineNum;

        //public HamlLine HamlLine
        //{
        //    get { return _line; }
        //}

        protected HamlNode(HamlLine nodeLine)
        {
            _line = nodeLine;
            _content = nodeLine.Content;
            _sourceFileLineNum = _line.SourceFileLineNo;
        }

        protected HamlNode(int sourceFileLineNum, string content)
        {
            _sourceFileLineNum = sourceFileLineNum;
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

        public int SourceFileLineNum
        {
            protected set
            {
                _sourceFileLineNum = value;
            }
            get
            {
                return _sourceFileLineNum;
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

        public HamlNode Previous { get; private set; }
        public HamlNode Next { get; private set; }
        public HamlNode Parent { get; private set; }

        private HamlNode PreviousNonWhitespaceNode()
        {
            var node = this.Previous;
            while (node != null && node.IsWhitespaceNode())
                node = node.Previous;
            return node;
        }

        private HamlNode NextNonWhitespaceNode()
        {
            var node = this.Next;
            while (node != null && node.IsWhitespaceNode())
                node = node.Next;
            return node;
        }

        public bool IsWhitespaceNode()
        {
            return (this is HamlNodeTextContainer)
                && ((HamlNodeTextContainer)this).IsWhitespace();
        }

        public bool IsLeadingWhitespaceTrimmed
        {
            get
            {
                if (IsSurroundingWhitespaceRemoved())
                    return true;

                var previousNonWhitespaceNode = PreviousNonWhitespaceNode();
                if (previousNonWhitespaceNode != null && previousNonWhitespaceNode.IsSurroundingWhitespaceRemoved())
                    return true;
                else if (previousNonWhitespaceNode == null)
                {
                    var parentNode = ParentNonWhitespaceNode();
                    if (parentNode != null && parentNode.IsInternalWhitespaceTrimmed())
                        return true;
                }
                return false;
            }
        }

        public bool IsTrailingWhitespaceTrimmed
        {
            get
            {
                var nextNonWhitespaceNode = NextNonWhitespaceNode();
                if (nextNonWhitespaceNode != null && nextNonWhitespaceNode.IsSurroundingWhitespaceRemoved())
                    return true;
                else if (nextNonWhitespaceNode == null)
                {
                    var parentNode = ParentNonWhitespaceNode();
                    if (parentNode != null && parentNode.IsInternalWhitespaceTrimmed())
                        return true;
                }
                return false;
            }
        }

        private bool IsInternalWhitespaceTrimmed()
        {
            return this is HamlNodeTag
                && ((HamlNodeTag)this).WhitespaceRemoval == WhitespaceRemoval.Internal;
        }

        private bool IsSurroundingWhitespaceRemoved()
        {
            return this is HamlNodeTag
                && ((HamlNodeTag)this).WhitespaceRemoval == WhitespaceRemoval.Surrounding;
        }

        private HamlNode ParentNonWhitespaceNode()
        {
            var parentNode = Parent;
            while (parentNode != null && parentNode.IsWhitespaceNode())
                parentNode = parentNode.Parent;
            return parentNode;
        }
    }
}
