using System;

namespace NHaml.Rules
{
    public class EofMarkupRule : MarkupRule
    {
        public static readonly string SignifierChar = Convert.ToChar( 26 ).ToString();

        public override string Signifier
        {
            get { return SignifierChar; }
        }

        public override BlockClosingAction Render( TemplateParser templateParser )
        {
            return EmptyClosingAction;
        }
    }
}