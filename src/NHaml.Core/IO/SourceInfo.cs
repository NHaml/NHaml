namespace NHaml.Core.IO
{
    public struct SourceInfo
    {
        /// <summary>
        /// Stores a position in the file
        /// </summary>
        /// <param name="lineNumber">The line number the position refers</param>
        /// <param name="index">The character position within that line</param>
        /// <param name="position">The character position for the whole file</param>
        public SourceInfo(int lineNumber, int index, int position)
            : this()
        {
            LineNumber = lineNumber;
            Index = index;
            Position = position;
        }

        public int LineNumber { get; set; }
        public int Index { get; set; }
        public int Position { get; set; }
    }
}