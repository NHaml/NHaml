using System;
using NHaml.Core.IO;

namespace NHaml.Core.Exceptions
{
    public class ParserException : Exception
    {
        public readonly CharacterReader Reader;

        static string GetMessage(CharacterReader cr, string message)
        {
            if (cr is InputReader)
            {
                var ir = cr as InputReader;
                return message + String.Format(" on line {0} character {1} (position {2})", ir.SourceInfo.LineNumber+1, ir.SourceInfo.Index, ir.SourceInfo.Position);
            }
            else
            {
                return message + String.Format(" on character position {0}", cr.Index);
            }
        }

        public ParserException(CharacterReader reader, string message)
            : base(GetMessage(reader,message))
        {
            Reader = reader;
        }
    }
}