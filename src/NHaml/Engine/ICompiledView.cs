namespace NHaml.Engine
{
  public interface ICompiledView<TView, TViewData>
  {
    TView CreateView();
    void RecompileIfNecessary(TViewData viewData);
  }
}