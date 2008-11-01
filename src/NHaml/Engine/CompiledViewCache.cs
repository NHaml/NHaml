using System;
using System.Collections.Generic;

namespace NHaml.Engine
{
  public delegate ICompiledView<TView> CompiledViewCreator<TView>();

  public delegate Type ViewBaseTypeObtainer();

  public class CompiledViewCache<TView>
  {
    private readonly TemplateCompiler _templateCompiler = new TemplateCompiler();
    private readonly Dictionary<string, ICompiledView<TView>> _viewCache = new Dictionary<string, ICompiledView<TView>>();

    public TemplateCompiler TemplateCompiler
    {
      get { return _templateCompiler; }
    }

    public TView GetView(CompiledViewCreator<TView> compiledViewCreator, string viewKey, ViewBaseTypeObtainer viewBaseTypeObtainer)
    {
      if (compiledViewCreator == null)
      {
        throw new ArgumentNullException("compiledViewCreator");
      }

      if (viewBaseTypeObtainer == null)
      {
        throw new ArgumentNullException("viewBaseTypeObtainer");
      }

      ICompiledView<TView> compiledView;

      if (!_viewCache.TryGetValue(viewKey, out compiledView))
      {
        lock (_viewCache)
        {
          if (!_viewCache.TryGetValue(viewKey, out compiledView))
          {
            _templateCompiler.ViewBaseType = viewBaseTypeObtainer();
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