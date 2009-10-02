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

        public bool Read()
        {
            if(!_reader.Read())
                return false;

            _markupRule = FindMarkupRule(_reader.Current.Text);
            Text = _reader.Current.Text;
            Indent = _reader.Current.Indent;

            return true;
        }

        public AstNode CreateNode()
        {
            return _markupRule != null ? _markupRule.Process(this) : new TextNode(Text);
        }

        public AstNode ParseChildren(int baseIdentation, AstNode currentChild)
        {
            var nodes = new List<AstNode>();

            while(Read() && baseIdentation < Indent)
            {
                var node = CreateNode();

                if(node != null)
                    nodes.Add(node);
            }

            if(nodes.Count>0)
            {
                if(currentChild!=null)
                    nodes.Insert(0,currentChild);

                //if(nodes.Count == 1)
                  //  return nodes[0];

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
    }
}