using System.IO;
using System.Text;
using System.Web.NHaml.Parser.Exceptions;

namespace System.Web.NHaml.IO
{
    public class HamlFileLexer
    {
        bool _eof;
        int _sourceFileLineIndex;
        private readonly HamlLineLexer _lineLexer;

        public HamlFileLexer()
        {
            _lineLexer = new HamlLineLexer();
        }

        public HamlFile Read(TextReader reader, string fileName)
        {
            _sourceFileLineIndex = 1;

            if (reader == null)
                throw new ArgumentNullException("reader");

            var result = new HamlFile(fileName);
            _eof = (reader.Peek() < 0);

            while (_eof == false)
                ReadHamlLine(reader, result);

            return result;
        }

        private void ReadHamlLine(TextReader reader, HamlFile result)
        {
            string currentLine = ReadLine(reader);
            while (_lineLexer.GetEndOfTagIndex(currentLine) < 0)
            {
                if (_eof)
                    throw new HamlMalformedTagException("Multi-line tag found with no end token.", _sourceFileLineIndex);
                currentLine += " " + ReadLine(reader);
            }

            result.AddRange(new HamlLineLexer().ParseHamlLine(currentLine, _sourceFileLineIndex - 1));
        }

        private string ReadLine(TextReader reader)
        {
            var line = new StringBuilder();
            int r = reader.Read();
            while (r >= 0)
            {
                char c = Convert.ToChar(r);
                if (c == '\n') break;
                if (c == '\r')
                {
                    HandleCrLfSequence(reader);
                    break;
                }

                line.Append(c);
                r = reader.Read();
            }
            if (r <= 0)
                _eof = true;
            _sourceFileLineIndex++;
            return line.ToString();
        }

        private static void HandleCrLfSequence(TextReader reader)
        {
            int peek = reader.Peek();
            if ((peek != -1) && (Convert.ToChar(peek) == '\n'))
                reader.Read();
        }
    }
}