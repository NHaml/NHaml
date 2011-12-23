using System;
using System.IO;
using System.Text;

namespace NHaml4.IO
{
    public class HamlFileLexer
    {
        bool _eof;

        public HamlFile Read(TextReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            var result = new HamlFile();

            while (_eof == false)
            {
                string currentLine = ReadLine(reader);
                //if ((!string.IsNullOrEmpty(currentLine)) || (_eof == false))
                result.AddLine(new HamlLine(currentLine));
            }

            return result;
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