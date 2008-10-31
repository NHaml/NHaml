using System.Diagnostics.CodeAnalysis;
using System.Security.Permissions;
using System.Web;

namespace NHaml.Engine
{
  [SuppressMessage("Microsoft.Design", "CA1005")]
  [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  public abstract class ViewEngine<TCompiledView, TContext, TView, TViewData>
    where TCompiledView : ICompiledView<TView, TViewData>
  {
    private readonly CompiledViewCache<TCompiledView, TView, TViewData> _viewCache
      = new CompiledViewCache<TCompiledView, TView, TViewData>();

    public TView FindAndCreateView(string viewName, string layoutName, TContext context)
    {
      return CacheView(viewName, context, () => CreateView(viewName, layoutName, context));
    }

    public TView FindAndCreatePartialView(string viewName, TContext context)
    {
      return CacheView(viewName, context, () => CreatePartialView(viewName, context));
    }

    private TView CacheView(string viewName, TContext context,
      CompiledViewCache<TCompiledView, TView, TViewData>.CreateCompiledViewDelegate createView)
    {
      var viewKey = GetViewKey(viewName, context);
      return _viewCache.GetView(createView, viewKey, () => GetViewData(context));
    }

    protected abstract string GetViewKey(string viewName, TContext context);
    protected abstract TViewData GetViewData(TContext context);
    protected abstract TCompiledView CreateView(string viewName, string layoutName, TContext context);
    protected abstract TCompiledView CreatePartialView(string viewName, TContext context);

    protected TemplateCompiler TemplateCompiler
    {
      get { return _viewCache.TemplateCompiler; }
    }
  }
}