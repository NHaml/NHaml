namespace NHaml.BackEnds
{
  public interface IAttributeRenderer
  {
    void Render(CompilationContext compilationContext, string attributes);
  }
}