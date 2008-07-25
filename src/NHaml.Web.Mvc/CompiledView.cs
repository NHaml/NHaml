using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Permissions;
using System.Web;
using System.Web.Mvc;

namespace NHaml.Web.Mvc
{
  [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  public class CompiledView
  {
    private readonly TemplateCompiler _templateCompiler;

    private readonly string _templatePath;
    private readonly string _layoutPath;

    private TemplateActivator<IMvcView> templateActivator;

    private readonly object _sync = new object();

    private readonly Dictionary<string, DateTime> _fileTimestamps
      = new Dictionary<string, DateTime>();

    public CompiledView(TemplateCompiler templateCompiler,
      string templatePath, string layoutPath, ViewDataDictionary viewData)
    {
      _templateCompiler = templateCompiler;
      _templatePath = templatePath;
      _layoutPath = layoutPath;

      CompileView(viewData);
    }

    public IMvcView CreateView()
    {
      return templateActivator();
    }

    public void RecompileIfNecessary(ViewDataDictionary viewData)
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

    private void CompileView(ViewDataDictionary viewData)
    {
      var modelType = typeof(object);

      if ((viewData != null) && (viewData.Model != null))
      {
        modelType = viewData.Model.GetType();
      }

      var inputFiles = new List<string>();

      templateActivator = _templateCompiler.Compile<IMvcView>(_templatePath, _layoutPath, inputFiles, modelType);

      foreach (var inputFile in inputFiles)
      {
        _fileTimestamps[inputFile] = File.GetLastWriteTime(inputFile);
      }
    }
  }
}