using NHaml.Compilers;

namespace NHaml.Rules
{
    public class SilentCommentMarkupRule : MarkupRule
    {
        public const string SignifierChar = "-#";

        public override string Signifier
        {
            get { return SignifierChar; }
        }

        public override bool PerformCloseActions
        {
            get { return false; }
        }

        public override BlockClosingAction Render(ViewSourceReader viewSourceReader, TemplateOptions options, TemplateClassBuilder builder)
        {
            int currentLineIndentCount = viewSourceReader.CurrentInputLine.IndentCount;
            while ((viewSourceReader.NextInputLine != null)
                && (viewSourceReader.NextInputLine.IndentCount > currentLineIndentCount))
            {
                viewSourceReader.MoveNext();
            }
            return null;
        }        
    }
}