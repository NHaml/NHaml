namespace NHaml
{
  public interface IAttributeRenderer
  {
    void Render(CompilationContext compilationContext, string attributes);
  }
}