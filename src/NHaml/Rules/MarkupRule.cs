namespace NHaml.Rules
{
  public abstract class MarkupRule
  {
    public abstract char Signifier { get; }

    public virtual bool MergeMultiline
    {
      get { return false; }
    }

    public abstract BlockClosingAction Render(TemplateParser templateParser);

    public virtual void Process(TemplateParser templateParser)
    {
      templateParser.CloseBlocks();
      templateParser.BlockClosingActions.Push(Render(templateParser) ?? delegate
        {
        });
      templateParser.MoveNext();
    }
  }
}