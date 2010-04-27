using System;

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



    }
}