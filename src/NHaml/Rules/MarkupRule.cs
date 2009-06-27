namespace NHaml.Rules
{
    public abstract class MarkupRule
    {
        public const string ErrorParsingTag = "Error parsing tag";
        public abstract string Signifier { get; }

        public static readonly BlockClosingAction EmptyClosingAction = () => { };
        public abstract BlockClosingAction Render( TemplateParser templateParser );

        public virtual void Process( TemplateParser templateParser )
        {
            templateParser.CloseBlocks();
            templateParser.BlockClosingActions.Push( Render( templateParser ));
            templateParser.MoveNext();
        }
    }
}