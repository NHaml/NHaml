using System;
using System.IO;

namespace NHaml
{
    public class TokenReader
    {
        TextReader textReader;
        public TokenReader(TextReader textReader)
        {
            this.textReader = textReader;
            current = ReadFromStream();
            if (!current.IsEnd)
            {
                next = ReadFromStream();
            }
        }

        private Token current;
        private Token next;

        public Token Peek()
        {
            return current;
        }

        public Token Read()
        {
            var temp = current;
            current = next;
            next = ReadFromStream();
            return temp;
        }
        private Token ReadFromStream()
        {
            var token = new Token();
            var charAsInt = textReader.Read();
            if (charAsInt == -1)
            {
                token.IsEnd = true;
            }
            else
            {
                var character = (char) charAsInt;
                if (character == '\\')
                {
                    charAsInt = textReader.Read();
                    if (charAsInt == -1)
                    {
                        throw new Exception("Last character cannot be an escape character '\\'.");
                    }
                    token.IsEscaped = true;
                    character = (char) charAsInt;
                }
                token.Character = character;
            }
            return token;
        }

        public Token Peek2()
        {
            return next;
        }
    }
}