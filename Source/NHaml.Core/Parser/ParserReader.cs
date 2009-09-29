using System;
using NHaml.Core.Ast;
using NHaml.Core.Parser.Rules;

namespace NHaml.Core.Parser
{
    public class ParserReader
    {
        private readonly InputLineReader _reader;
        private readonly MarkupRuleBase[] _rules;

        public ParserReader(MarkupRuleBase[] rules, InputLineReader reader)
        {
            if(reader == null)
                throw new ArgumentNullException("reader");

            _rules = rules;
            _reader = reader;
        }

        public string Text { get; private set; }

        public int Indent { get; private set; }

        public MarkupRuleBase MarkupRule { get; private set; }

        public bool Read()
        {
            if(!_reader.Read())
                return false;

            MarkupRule = FindMarkupRule(_reader.Current.Text);
            Text = _reader.Current.Text;
            Indent = _reader.Current.Indent;

            return true;
        }

        public AstNode CreateNode()
        {
            return MarkupRule != null ? MarkupRule.Process(this) : new TextNode(Text);
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