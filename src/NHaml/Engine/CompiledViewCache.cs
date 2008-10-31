using System;
using System.Collections.Generic;

namespace NHaml.Engine
{
  public class CompiledViewCache<TView>
  {
    public delegate ICompiledView<TView> CreateCompiledViewDelegate();

    public delegate Type ObtainViewBaseTypeDelegate();

    private readonly TemplateCompiler _templateCompiler = new TemplateCompiler();
    private readonly Dictionary<string, ICompiledView<TView>> _viewCache = new Dictionary<string, ICompiledView<TView>>();

    public TemplateCompiler TemplateCompiler
    {
      get { return _templateCompiler; }
    }

    public TView GetView(CreateCompiledViewDelegate createView, string viewKey, ObtainViewBaseTypeDelegate obtainViewBaseType)
    {
      if (createView == null)
      {
        throw new ArgumentNullException("createView");
      }

      if (obtainViewBaseType == null)
      {
        throw new ArgumentNullException("obtainViewBaseType");
      }

      ICompiledView<TView> compiledView;

      if (!_viewCache.TryGetValue(viewKey, out compiledView))
      {
        lock (_viewCache)
        {
          if (!_viewCache.TryGetValue(viewKey, out compiledView))
          {
            _templateCompiler.ViewBaseType = obtainViewBaseType();
            compiledView = createView();

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