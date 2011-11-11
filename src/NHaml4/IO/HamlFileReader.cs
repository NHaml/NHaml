using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace NHaml.IO
{
    public class HamlFileReader
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
                if ((!string.IsNullOrEmpty(currentLine)) || (_eof == false))
                    result.AddLine(currentLine);
            }

            return result;
            //var result = new List<InputLine>();
            //int currentLine = 0;
            //InputLine line;
            //do
            //{
            //    line = ReadLine(currentLine++);
            //    if (line != null) result.Add(line);
            //}
            //while (!string.IsNullOrEmpty(line))
            //{
                
            //}
        }

        private string ReadLine(TextReader reader)
        {
            StringBuilder line = new StringBuilder();
            int r = reader.Read();
            while (r >= 0)
            {
                char c = Convert.ToChar(r);
                line.Append(c);
                if (c == '\n') break;
                if (c == '\r')
                {
                    int peek = reader.Peek();
                    if (peek == -1) break;
                    if (Convert.ToChar(peek) != '\n') break;
                }
                r = reader.Read();
            }
            if (r <= 0)
                _eof = true;
            return line.ToString();

            //_inputLineEndPosition = _inputPosition + line.Length;

            throw new NotImplementedException();
            //_oldInputPosition = _inputPosition;
            //_inputPosition = _inputLineEndPosition;
            //StringBuilder line = new StringBuilder();
            //int r = _reader.Read();
            //while (r >= 0)
            //{
            //    char c = Convert.ToChar(r);
            //    line.Append(c);
            //    if (c == '\n') break;
            //    if (c == '\r')
            //    {
            //        int peek = _reader.Peek();
            //        if (peek == -1) break;
            //        if (Convert.ToChar(peek) != '\n') break;
            //    }
            //    r = _reader.Read();
            //}
            //_inputLineEndPosition = _inputPosition + line.Length;
            //return line.ToString();
        }
    }
}