namespace NHaml.Rules
{
  public abstract class MarkupRule
  {
    public abstract char Signifier { get; }

    public virtual bool MergeMultiline
    {
      get { return false; }
    }

    public abstract BlockClosingAction Render(CompilationContext compilationContext);

    public virtual void Process(CompilationContext compilationContext)
    {
      compilationContext.CloseBlocks();
      compilationContext.BlockClosingActions.Push(Render(compilationContext) ?? delegate
        {
        });
      compilationContext.MoveNext();
    }
  }
}