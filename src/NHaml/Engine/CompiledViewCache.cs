using System;
using System.Collections.Generic;

using NHaml.Utils;

namespace NHaml.Engine
{
  public delegate ICompiledView<TView> CompiledViewCreator<TView>();

  public class CompiledViewCache<TView>
  {
    private readonly TemplateCompiler _templateCompiler = new TemplateCompiler();

    private readonly Dictionary<string, ICompiledView<TView>> _viewCache
      = new Dictionary<string, ICompiledView<TView>>();

    public TemplateCompiler TemplateCompiler
    {
      get { return _templateCompiler; }
    }

    public TView GetView(CompiledViewCreator<TView> compiledViewCreator, 
      string viewKey, Type viewBaseType)
    {
      Invariant.ArgumentNotNull(compiledViewCreator, "compiledViewCreator");
      Invariant.ArgumentNotEmpty(viewKey, "viewKey");
      Invariant.ArgumentNotNull(viewBaseType, "viewBaseType");

      ICompiledView<TView> compiledView;

      if (!_viewCache.TryGetValue(viewKey, out compiledView))
      {
        lock (_viewCache)
        {
          if (!_viewCache.TryGetValue(viewKey, out compiledView))
          {
            _templateCompiler.ViewBaseType = viewBaseType;

            compiledView = compiledViewCreator();

            _viewCache.Add(viewKey, compiledView);
          }
        }
      }

      if (!_templateCompiler.IsProduction)
      {
        compiledView.RecompileIfNecessary();
      }

      return compiledView.CreateView();
    }
  }
}