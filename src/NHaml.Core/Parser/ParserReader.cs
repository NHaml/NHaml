using System;
using System.Collections.Generic;
using System.IO;
using NHaml.Core.Ast;
using NHaml.Core.Parser.Rules;
using NHaml.Core.IO;

namespace NHaml.Core.Parser
{
    public class ParserReader
    {
        private readonly List<MarkupRuleBase> _rules;
        private MarkupRuleBase _markupRule;

        public ParserReader(List<MarkupRuleBase> rules, InputReader reader)
        {
            if(reader == null)
                throw new ArgumentNullException("reader");

            _rules = rules;
            Input = reader;
        }

        public int LineNumber { get; private set; }

        public string Text { get; private set; }

        public int Indent { get; private set; }

        public int Index { get; private set; }

        public InputReader Input { get; private set; }

        public bool Read()
        {
            if(!Input.ReadNextLine())
                return false;

            _markupRule = FindMarkupRule(Input.CurrentLine.Text);
            Text = Input.CurrentLine.Text;
            Index = Input.CurrentLine.StartIndex;
            LineNumber = Input.CurrentLine.LineNumber;
            Indent = Input.CurrentLine.Indent;

            return true;
        }

        public TextNode ParseText( string text, int index )
        {
            return new TextParser(new CharacterReader(text,index), Input.CurrentLine.EscapeLine).Parse();
        }

        public AstNode ParseNode()
        {
            return _markupRule != null ? _markupRule.Process(this) : ParseText(Text, 0);
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

                if (Input.NextLine != null && Input.NextLine.Indent <= baseIdentation)
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