using System;
using NHaml.Core.Parser.Rules;

namespace NHaml.Core.Parser
{
    public class ParserContext
    {
        public ParserContext(Parser parser, InputLineReader reader)
        {
            if(parser == null)
                throw new ArgumentNullException("parser");
            if(reader == null)
                throw new ArgumentNullException("reader");

            Parser = parser;
            Reader = reader;
        }

        public Parser Parser { get; private set; }
        public InputLineReader Reader { get; private set; }
    }
}