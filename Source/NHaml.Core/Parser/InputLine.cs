namespace NHaml.Core.Parser
{
    public class InputLine
    {
        private readonly string _line;
        private readonly int _lineNumber;

        public InputLine(string line, int lineNumber)
        {
            _line = line;
            _lineNumber = lineNumber;
        }

        public int LineNumber
        {
            get { return _lineNumber; }
        }

        public override string ToString()
        {
            return _line;
        }
    }
}