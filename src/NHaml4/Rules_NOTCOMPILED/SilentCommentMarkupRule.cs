using NHaml.Compilers;
using NHaml.Parser;

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

        public override BlockClosingAction Render(HamlNode node, TemplateOptions options, TemplateClassBuilder builder)
        {
            //int currentLineIndentCount = viewSourceReader.CurrentInputLine.IndentCount;
            //while ((viewSourceReader.NextInputLine != null)
            //    && (viewSourceReader.NextInputLine.IndentCount > currentLineIndentCount))
            //{
            //    viewSourceReader.MoveNext();
            //}
            return null;
        }        
    }
}