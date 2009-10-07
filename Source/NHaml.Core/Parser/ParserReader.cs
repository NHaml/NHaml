using System;
using System.Collections.Generic;
using NHaml.Core.Ast;
using NHaml.Core.Parser.Rules;

namespace NHaml.Core.Parser
{
    public class ParserReader
    {
        private readonly InputLineReader _reader;
        private readonly MarkupRuleBase[] _rules;
        private MarkupRuleBase _markupRule;

        public ParserReader(MarkupRuleBase[] rules, InputLineReader reader)
        {
            if(reader == null)
                throw new ArgumentNullException("reader");

            _rules = rules;
            _reader = reader;
        }

        public string Text { get; private set; }

        public int Indent { get; private set; }

        public bool IsEndOfStream
        {
            get { return _reader.Current == null; }
        }

        public bool Read()
        {
            if(!_reader.Read())
                return false;

            _markupRule = FindMarkupRule(_reader.Current.Text);
            Text = _reader.Current.Text;
            Indent = _reader.Current.Indent;

            return true;
        }

        public AstNode ParseNode()
        {
            return _markupRule != null ? _markupRule.Process(this) : ParseText(Text, 0);
        }

        public TextNode ParseText(string text, int offset)
        {
            return new TextParser(new CharacterReader(text, 0)).Parse();
        }

        public AstNode ParseChildren(int baseIdentation, AstNode currentChild)
        {
            return ParseChildren(baseIdentation, currentChild, ParseNode);
        }

        public AstNode ParseLines(int baseIdentation, AstNode currentChild)
        {
            return ParseChildren(baseIdentation, currentChild, () => ParseText(Text, 0));
        }

        private AstNode ParseChildren(int baseIdentation, AstNode currentChild, ParseActionDelegate parser)
        {
            var nodes = new List<AstNode>();

            while(Read())
            {
                var node = parser();

                if(node != null)
                    nodes.Add(node);

                if(_reader.Next != null && _reader.Next.Indent <= baseIdentation)
                    break;
            }

            if(nodes.Count > 0)
            {
                if(currentChild != null)
                    nodes.Insert(0, currentChild);

                return new ChildrenNode(nodes);
            }

            return currentChild;
        }

        private MarkupRuleBase FindMarkupRule(string text)
        {
            foreach(var rule in _rules)
                if(rule.IsMatch(text))
                    return rule;

            return null;
        }

        private delegate AstNode ParseActionDelegate();
    }
}