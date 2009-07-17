using System;
using System.Collections.Generic;

namespace NHaml
{
    public class Token
    {
        public char Character { get; set; }
        public bool IsWhiteSpace
        {
            get
            {
                return Char.IsWhiteSpace(Character);
            }
        }
        public bool IsEnd { get; set; }

        public bool IsEscaped { get; set; }



        public static LinkedList<Token> ReadAllTokens(string attributeString)
        {
            var list = new LinkedList<Token>();
            for (var index = 0; index < attributeString.Length; index++)
            {
                var character = attributeString[index];
                var token = new Token();
                if (character == '\\')
                {
                    index = index + 1;
                    if (index == attributeString.Length)
                    {
                        throw new Exception();
                    }
                    token.IsEscaped = true;
                }
                token.Character = attributeString[index];
                list.AddLast(token);
            }
            list.AddLast(new Token { IsEnd = true });
            return list;

        }
    }


}