namespace NHaml.Rules
{
    public class IdMarkupRule : TagMarkupRule
    {
        public override char Signifier
        {
            get { return '#'; }
        }

        protected override string PreprocessLine( InputLine inputLine )
        {
            return string.Format("div#{0}", inputLine.NormalizedText);
        }
    }
}