using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Security.Permissions;
using System.Web;

namespace NHaml.Engine
{
  [SuppressMessage("Microsoft.Design", "CA1005")]
  [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  public abstract class ViewEngine<TCompiledView, TViewContext, TView, TViewData>
    where TCompiledView : CompiledView<TView, TViewData>
  {
    private readonly Dictionary<string, TCompiledView> _viewCache
      = new Dictionary<string, TCompiledView>();

    private readonly TemplateCompiler _templateCompiler
      = new TemplateCompiler();

    public void RenderView(TViewContext viewContext)
    {
      var viewKey = GetViewKey(viewContext);

      TCompiledView compiledView;

      if (!_viewCache.TryGetValue(viewKey, out compiledView))
      {
        lock (_viewCache)
        {
          if (!_viewCache.TryGetValue(viewKey, out compiledView))
          {
            compiledView = CreateView(viewContext);

            _viewCache.Add(viewKey, compiledView);
          }
        }
      }

      if (!_templateCompiler.IsProduction)
      {
        compiledView.RecompileIfNecessary(GetViewData(viewContext));
      }

      var view = compiledView.CreateView();

      RenderView(view, viewContext);
    }

    protected abstract string GetViewKey(TViewContext viewContext);
    protected abstract TViewData GetViewData(TViewContext viewContext);
    protected abstract TCompiledView CreateView(TViewContext viewContext);
    protected abstract void RenderView(TView view, TViewContext viewContext);
    protected abstract string SelectLayout(TViewContext viewContext);

    protected TemplateCompiler TemplateCompiler
    {
      get { return _templateCompiler; }
    }
  }
}