using System;
using System.Collections.Generic;
using System.Text;
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

        public AstNode CreateNode()
        {
            return _markupRule != null ? _markupRule.Process(this) : ParseText(Text);
        }

        public AstNode ParseChildren(int baseIdentation, AstNode currentChild)
        {
            var nodes = new List<AstNode>();

            while(Read())
            {
                var node = CreateNode();

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

        public AstNode ParseLines(int baseIdentation, AstNode currentChild)
        {
            var nodes = new List<AstNode>();

            while(Read())
            {
                var node = ParseText(new CharacterReader(Text));

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

        public TextNode ParseText(string text)
        {
            return ParseText(new CharacterReader(text));
        }

        public TextNode ParseText(CharacterReader reader)
        {
            //Todo: extract to seperate class
            var node = new TextNode();

            var buffer = new StringBuilder();
            while(reader.Read())
                switch(reader.Current)
                {
                    case '\\':
                    {
                        if(reader.Next == '#')
                            reader.Read(); // escaped # - eat \
                        else if(reader.Next == '\\')
                            reader.Read(); // escaped \ - eat \

                        goto default;
                    }
                    case '#':
                    {
                        if(reader.Next == '{')
                        {
                            reader.Read(); // eat #

                            if(buffer.Length > 0)
                            {
                                node.Chunks.Add(new TextChunk(buffer.ToString()));
                                buffer = new StringBuilder();
                            }

                            while(reader.Read() && reader.Current != '}')
                            {
                                if(reader.Current == '\\')
                                    if(reader.Next == '}')
                                        reader.Read(); // escaped } - eat \
                                    else if(reader.Next == '\\')
                                        reader.Read(); // escaped \ - eat \

                                buffer.Append(reader.Current);
                            }

                            if(buffer.Length > 0)
                            {
                                node.Chunks.Add(new CodeChunk(buffer.ToString()));
                                buffer = new StringBuilder();
                            }

                            continue;
                        }

                        goto default;
                    }
                    default:
                    {
                        buffer.Append(reader.Current);
                        break;
                    }
                }

            if(buffer.Length > 0)
                node.Chunks.Add(new TextChunk(buffer.ToString()));

            if(node.Chunks.Count == 0)
                return null;

            return node;
        }
    }
}