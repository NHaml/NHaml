using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Permissions;
using System.Web;

namespace NHaml.Engine
{
  [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  public class CompiledView<TView> : ICompiledView<TView>
  {
    private readonly TemplateCompiler _templateCompiler;

    private readonly string _templatePath;
    private readonly string _layoutPath;

    private TemplateActivator<TView> _templateActivator;

    private readonly object _sync = new object();

    private readonly Dictionary<string, DateTime> _fileTimestamps
      = new Dictionary<string, DateTime>();

    public CompiledView(TemplateCompiler templateCompiler,
      string templatePath, string layoutPath)
    {
      _templateCompiler = templateCompiler;
      _templatePath = templatePath;
      _layoutPath = layoutPath;

      CompileView();
    }

    public TView CreateView()
    {
      return _templateActivator();
    }

    public void RecompileIfNecessary()
    {
      lock (_sync)
      {
        foreach (var inputFile in _fileTimestamps)
        {
          if (File.GetLastWriteTime(inputFile.Key) > inputFile.Value)
          {
            CompileView();

            break;
          }
        }
      }
    }

    private void CompileView()
    {
      var inputFiles = new List<string>();

      _templateActivator = _templateCompiler
        .Compile<TView>(_templatePath, _layoutPath, inputFiles);

      foreach (var inputFile in inputFiles)
      {
        _fileTimestamps[inputFile] = File.GetLastWriteTime(inputFile);
      }
    }

  }
}