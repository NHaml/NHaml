using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NHaml.Exceptions;

namespace NHaml
{
    public class ExpressionStringParser2
    {
        private readonly LinkedList<Token> Tokens;

        private string current;
        private bool? isExpression;

        public List<ExpressionStringToken> ExpressionStringTokens { get; protected set; }


        public ExpressionStringParser2(string expressionString)
        {
            Tokens = Token.ReadAllTokens(expressionString);
            ExpressionStringTokens = new List<ExpressionStringToken>(); 
        }

        public void Parse()
        {
            ExpressionStringTokens.Clear();
            Token next;
            while (!(next=Tokens.First.Value).IsEnd)
            {
                var character = next.Character;
                if (character == '#' && Tokens.First.Next.Value.Character == '{')
                {
                    ProcessHashedCode();
                }
                else
                {
                    ProcessString();
                }
                ExpressionStringTokens.Add(new ExpressionStringToken(current,isExpression.Value));
                current = null;
                isExpression = null;
            }

        }

        private void ProcessString()
        {
            isExpression = false;
            while (true)
            {
                var token = Tokens.First.Value;
                Tokens.RemoveFirst();
                var character = token.Character;

                current += character;
                var next = Tokens.First;
                var nextValue = next.Value;
                if (nextValue.IsEnd)
                {
                    return;
                }

                if (nextValue.Character == '#' && next.Next.Value.Character == '{')
                {
                    return;
                }
            }
        }

        private void ProcessHashedCode()
        {
            isExpression = true;
            Tokens.RemoveFirst();
            Tokens.RemoveFirst();
            while (true)
            {
                var token = Tokens.First.Value;
                Tokens.RemoveFirst();
                current += token.Character;

                var next = Tokens.First.Value;
                if (next.IsEnd)
                {
                    throw new SyntaxException();
                }
                if (!next.IsEscaped && (next.Character == '}'))
                {
                    Tokens.RemoveFirst();
                    return;
                }
            }
        }

    }
}