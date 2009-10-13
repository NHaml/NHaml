namespace NHaml.Core.Ast
{
    public struct SourceInfo
    {
        public SourceInfo(int lineNumber, int index)
            : this()
        {
            LineNumber = lineNumber;
            Index = index;
        }

        public int LineNumber { get; set; }
        public int Index { get; set; }
    }
}