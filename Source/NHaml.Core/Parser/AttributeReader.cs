using System;

namespace NHaml.Core.Parser
{
    public class AttributeReader
    {
        private readonly CharacterReader _reader;
        private readonly ParserReader _parser;

        public AttributeReader(CharacterReader reader, ParserReader parser)
        {
            if(reader == null)
                throw new ArgumentNullException("reader");
            if(parser == null)
                throw new ArgumentNullException("parser");

            _reader = reader;
            _parser = parser;
        }
    }
}