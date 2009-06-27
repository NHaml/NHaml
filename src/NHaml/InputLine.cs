using System.Text.RegularExpressions;

using NHaml.Exceptions;

namespace NHaml
{
    public sealed class InputLine
    {
        private static readonly Regex _indentRegex
          = new Regex( @"^(\s*|\t*)", RegexOptions.Compiled | RegexOptions.Singleline );

        private static readonly Regex _multiLineRegex
          = new Regex( @"^.+\s+(\|\s*)$", RegexOptions.Compiled | RegexOptions.Singleline );

        private readonly int _indentSize;



        public InputLine( string text, int lineNumber )
            : this( text,  lineNumber, 2 )
        {
        }

        public InputLine( string text, int lineNumber, int indentSize )
        {
            Text = text;
            LineNumber = lineNumber;
            _indentSize = indentSize;

            var match = _multiLineRegex.Match( Text );

            IsMultiline = match.Success;

            if( IsMultiline )
            {
                Text = Text.Remove( match.Groups[1].Index );
            }

            //NormalizedText = Text.TrimStart();

            //if( !string.IsNullOrEmpty( NormalizedText ) )
            //{
            //    NormalizedText = NormalizedText.Remove( 0, 1 );
            //}

            Indent = _indentRegex.Match( Text ).Groups[0].Value;
            IndentCount = Indent.Length / _indentSize;
        }

    //    public char Signifier { get; private set; }

        public string Text { get; private set; }

       // public string Source { get; private set; }

       public string NormalizedText { get; set; }



        public string Indent { get; private set; }

        public int IndentCount { get; private set; }

        public int LineNumber { get; private set; }

        public bool IsMultiline { get; private set; }

        public void Merge( InputLine nextInputLine )
        {
            Text += nextInputLine.Text.TrimStart();
        }

        public void TrimEnd()
        {
            Text = Text.TrimEnd();
        }

        public void ValidateIndentation()
        {
            if( (Indent.Length % _indentSize) != 0 )
            {
                SyntaxException.Throw(this, "Illegal Indentation: Only {0} space character(s) allowed as indentation", _indentSize);
            }
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", LineNumber, Text);
        }
    }
}