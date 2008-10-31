using System;
using System.Collections.Generic;

namespace NHaml.Engine
{
  public class CompiledViewCache<TCompiledView, TView, TViewData>
    where TCompiledView : ICompiledView<TView, TViewData>
  {
    public delegate TCompiledView CreateCompiledViewDelegate();

    public delegate TViewData ObtainViewDataDelegate();

    private readonly TemplateCompiler _templateCompiler = new TemplateCompiler();
    private readonly Dictionary<string, TCompiledView> _viewCache = new Dictionary<string, TCompiledView>();

    public TemplateCompiler TemplateCompiler
    {
      get { return _templateCompiler; }
    }

    public TView GetView(CreateCompiledViewDelegate createView, string viewKey, ObtainViewDataDelegate obtainViewData)
    {
      if (createView == null)
      {
        throw new ArgumentNullException("createView");
      }

      if (obtainViewData == null)
      {
        throw new ArgumentNullException("obtainViewData");
      }

      TCompiledView compiledView;

      if (!_viewCache.TryGetValue(viewKey, out compiledView))
      {
        lock (_viewCache)
        {
          if (!_viewCache.TryGetValue(viewKey, out compiledView))
          {
            compiledView = createView();

            _viewCache.Add(viewKey, compiledView);
          }
        }
      }

      if (!_templateCompiler.IsProduction)
      {
        compiledView.RecompileIfNecessary(obtainViewData());
      }

      return compiledView.CreateView();
    }
  }
}