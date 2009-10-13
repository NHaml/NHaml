namespace NHaml.Core.Parser
{
    public class InputLine
    {
        public InputLine(string text, int lineNumber, int indent, int startIndex, bool isMultiLine)
        {
            Text = text;
            LineNumber = lineNumber;
            Indent = indent;
            StartIndex = startIndex;
            IsMultiLine = isMultiLine;
        }

        public int StartIndex { get; private set; }
        
        public bool IsMultiLine { get; private set; }

        public int Indent { get; private set; }

        public string Text { get; private set; }

        public int LineNumber { get; private set; }

        public override string ToString()
        {
            return string.Format("{0}: {1}", LineNumber, Text);
        }
    }
}