using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Security.Permissions;
using System.Web;

namespace NHaml.Engine
{
  [SuppressMessage("Microsoft.Design", "CA1005")]
  [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  public abstract class ViewEngine<TCompiledView, TContext, TView, TViewData>
    where TCompiledView : CompiledView<TView, TViewData>
  {
    private readonly Dictionary<string, TCompiledView> _viewCache
      = new Dictionary<string, TCompiledView>();

    private readonly TemplateCompiler _templateCompiler
      = new TemplateCompiler();

    public TView FindAndCreateView(string viewName, string layoutName, TContext context)
    {
        return CacheView( viewName, context, () => CreateView(viewName, layoutName, context));
    }

    public TView FindAndCreatePartialView( string viewName, TContext context )
    {
        return CacheView( viewName, context, () => CreatePartialView(viewName, context));
    }

    private TView CacheView( string viewName, TContext context, CreateCompiledViewDelegate createView )
    {
        if( createView == null )
            throw new ArgumentNullException( "createView" );

        var viewKey = GetViewKey( viewName, context );

        TCompiledView compiledView;

        if( !_viewCache.TryGetValue( viewKey, out compiledView ) )
        {
            lock( _viewCache )
            {
                if( !_viewCache.TryGetValue( viewKey, out compiledView ) )
                {
                    compiledView = createView();

                    _viewCache.Add( viewKey, compiledView );
                }
            }
        }

        if( !_templateCompiler.IsProduction )
        {
            compiledView.RecompileIfNecessary( GetViewData( context ) );
        }

        return compiledView.CreateView();

    }

    private delegate TCompiledView CreateCompiledViewDelegate();
    protected abstract string GetViewKey( string viewName, TContext context );
    protected abstract TViewData GetViewData(TContext context);
    protected abstract TCompiledView CreateView( string viewName, string layoutName, TContext context );
    protected abstract TCompiledView CreatePartialView( string viewName, TContext context );

    protected TemplateCompiler TemplateCompiler
    {
      get { return _templateCompiler; }
    }
  }
}