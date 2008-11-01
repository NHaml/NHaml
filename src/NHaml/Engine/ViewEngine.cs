using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.Permissions;
using System.Web;

namespace NHaml.Engine
{
  [SuppressMessage("Microsoft.Design", "CA1005")]
  [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  public abstract class ViewEngine<TContext, TView>
  {
    private readonly CompiledViewCache<TView> _viewCache
      = new CompiledViewCache<TView>();

    protected abstract string GetViewKey(string viewName, TContext context);
    protected abstract Type GetViewBaseType(TContext context);
    protected abstract ICompiledView<TView> CreateView(string viewName, string layoutName, TContext context);
    protected abstract ICompiledView<TView> CreatePartialView(string viewName, TContext context);

    protected TemplateCompiler TemplateCompiler
    {
      get { return _viewCache.TemplateCompiler; }
    }

    public TView FindAndCreateView(string viewName, string layoutName, TContext context)
    {
      return CacheView(viewName, context, () => CreateView(viewName, layoutName, context));
    }

    public TView FindAndCreatePartialView(string viewName, TContext context)
    {
      return CacheView(viewName, context, () => CreatePartialView(viewName, context));
    }

    private TView CacheView(string viewName, TContext context, CompiledViewCreator<TView> compiledViewCreator)
    {
      var viewKey = GetViewKey(viewName, context);

      return _viewCache.GetView(compiledViewCreator, viewKey, GetViewBaseType(context));
    }
  }
}