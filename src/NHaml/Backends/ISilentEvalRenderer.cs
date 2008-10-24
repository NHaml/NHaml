namespace NHaml.Backends
{
  public interface ISilentEvalRenderer
  {
    BlockClosingAction Render(CompilationContext compilationContext);
  }
}