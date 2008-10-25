namespace NHaml.BackEnds
{
  public interface ISilentEvalRenderer
  {
    BlockClosingAction Render(CompilationContext compilationContext);
  }
}