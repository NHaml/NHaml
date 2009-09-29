namespace NHaml.Core.Parser
{
    public class InputLine
    {
        public InputLine(string text, int lineNumber, int indent, bool isMultiline)
        {
            Text = text;
            LineNumber = lineNumber;
            Indent = indent;
            IsMultiline = isMultiline;
        }

        public int Indent { get; private set; }
        
        public bool IsMultiline { get; private set; }

        public string Text { get; private set; }

        public int LineNumber { get; private set; }

        public override string ToString()
        {
            return string.Format("{0}: {1}", LineNumber, Text);
        }
    }
}