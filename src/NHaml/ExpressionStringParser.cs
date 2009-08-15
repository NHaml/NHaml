using System;
using System.Collections.Generic;
using System.IO;
using NHaml.Exceptions;

namespace NHaml
{
    public class ExpressionStringParser : IDisposable
    {
        private TokenReader tokenReader;

        private string current;
        private bool? isExpression;

        StringReader stringReader;
        public List<ExpressionStringToken> ExpressionStringTokens { get; protected set; }


        public ExpressionStringParser(string expressionString)
        {
            stringReader = new StringReader(expressionString);
            tokenReader = new TokenReader(stringReader);
            ExpressionStringTokens = new List<ExpressionStringToken>();
        }

        public void Parse()
        {
            ExpressionStringTokens.Clear();
            //HACK:
            if (tokenReader.Peek().IsEnd)
            {
                ExpressionStringTokens.Add(new ExpressionStringToken(string.Empty, false));
                return;
            }
            Token next;
            while (!(next = tokenReader.Peek()).IsEnd)
            {
                var character = next.Character;
                if (character == '#' && tokenReader.Peek2().Character == '{')
                {
                    ProcessHashedCode();
                }
                else
                {
                    ProcessString();
                }
                ExpressionStringTokens.Add(new ExpressionStringToken(current, isExpression.Value));
                current = null;
                isExpression = null;
            }

        }

        private void ProcessString()
        {
            isExpression = false;
            current = string.Empty;
            while (true)
            {
                var nextValue = tokenReader.Peek();
                if (nextValue.IsEnd)
                {
                    return;
                }

                if (nextValue.Character == '#' && tokenReader.Peek2().Character == '{')
                {
                    return;
                }
                var token = tokenReader.Read();
                var character = token.Character;

                current += character;
            }
        }

        private void ProcessHashedCode()
        {
            isExpression = true;
            tokenReader.Read();
            tokenReader.Read();
            while (true)
            {
                var token = tokenReader.Read();
                current += token.Character;

                var next = tokenReader.Peek();
                if (next.IsEnd)
                {
                    throw new SyntaxException();
                }
                if (!next.IsEscaped && (next.Character == '}'))
                {
                    tokenReader.Read();
                    return;
                }
            }
        }

        public void Dispose()
        {
            if (stringReader != null)
            {
                stringReader.Dispose();
            }
        }
    }
}