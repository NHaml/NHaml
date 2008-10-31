namespace NHaml.Engine
{
  public interface ICompiledView<TView>
  {
    TView CreateView();
    void RecompileIfNecessary();
  }
}