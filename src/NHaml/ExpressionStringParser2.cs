using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NHaml
{
    public class ExpressionStringParser2
    {
        private readonly LinkedList<Token> _expressionString;

        public List<ExpressionStringToken> Tokens { get; protected set; }


        public ExpressionStringParser2(LinkedList<Token> expressionString)
        {
            _expressionString = expressionString;
            Tokens = new List<ExpressionStringToken>(); 
        }

        public void Parse()
        {
            Tokens.Clear();


            Token next;
            while (!(next = _expressionString.First.Value).IsEnd)
            {

                var character = next.Value.Character;
                if (character == '#' && next.Next.Value.Character == '{')
                {
                    ProcessHashedCode();
                    return;
                }
                ProcessKey();
                AddCurrent();
                currentValue = null;
                currentKey = null;
            }

        }
    }
}