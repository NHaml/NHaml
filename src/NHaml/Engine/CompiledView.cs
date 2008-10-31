using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Permissions;
using System.Web;

namespace NHaml.Engine
{
  [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  public class CompiledView<TView, TViewData> : ICompiledView<TView, TViewData>
  {
    private readonly TemplateCompiler _templateCompiler;

    private readonly string _templatePath;
    private readonly string _layoutPath;

    private TemplateActivator<TView> _templateActivator;

    private readonly object _sync = new object();

    private readonly Dictionary<string, DateTime> _fileTimestamps
      = new Dictionary<string, DateTime>();

    public CompiledView(TemplateCompiler templateCompiler,
      string templatePath, string layoutPath, TViewData viewData)
    {
      _templateCompiler = templateCompiler;
      _templatePath = templatePath;
      _layoutPath = layoutPath;

      CompileView(viewData);
    }

    public TView CreateView()
    {
      return _templateActivator();
    }

    public void RecompileIfNecessary(TViewData viewData)
    {
      lock (_sync)
      {
        foreach (var inputFile in _fileTimestamps)
        {
          if (File.GetLastWriteTime(inputFile.Key) > inputFile.Value)
          {
            CompileView(viewData);

            break;
          }
        }
      }
    }

    private void CompileView(TViewData viewData)
    {
      var inputFiles = new List<string>();

      _templateActivator = _templateCompiler
        .Compile<TView>(_templatePath, _layoutPath, inputFiles, GetGenericArguments(viewData));

      foreach (var inputFile in inputFiles)
      {
        _fileTimestamps[inputFile] = File.GetLastWriteTime(inputFile);
      }
    }

    protected virtual Type[] GetGenericArguments(TViewData viewData)
    {
      return new Type[] {};
    }
  }
}