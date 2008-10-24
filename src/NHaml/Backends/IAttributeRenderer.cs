namespace NHaml.Backends
{
  public interface IAttributeRenderer
  {
    void Render(CompilationContext compilationContext, string attributes);
  }
}