using System;

namespace NHaml.Core.Exceptions
{
    public class ParserException : Exception
    {
        public ParserException(string message)
            : base(message)
        {
        }
    }
}