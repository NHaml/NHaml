using System.Collections.Generic;
using System.Data.Linq;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Security.Permissions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI;

using NHaml.Configuration;

namespace NHaml.Web.Mvc
{
  [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  public class NHamlViewEngine : IViewEngine
  {
    private static readonly Dictionary<string, CompiledView> _viewCache
      = new Dictionary<string, CompiledView>();

    private static readonly TemplateCompiler _templateCompiler
      = new TemplateCompiler();

    private static bool _production;

    [SuppressMessage("Microsoft.Performance", "CA1810")]
    static NHamlViewEngine()
    {
      _templateCompiler.AddUsing("System.Web");
      _templateCompiler.AddUsing("System.Web.Mvc");
      _templateCompiler.AddUsing("System.Web.Routing");

      _templateCompiler.AddUsing("NHaml.Web.Mvc");

      _templateCompiler.ViewBaseType = typeof(MvcView<>);

      _templateCompiler.AddReference(typeof(UserControl).Assembly.Location);
      _templateCompiler.AddReference(typeof(RouteValueDictionary).Assembly.Location);
      _templateCompiler.AddReference(typeof(DataContext).Assembly.Location);
      _templateCompiler.AddReference(typeof(TextInputExtensions).Assembly.Location);

      LoadConfiguration();
    }

    private static void LoadConfiguration()
    {
      var section = NHamlSection.Read();

      if (section != null)
      {
        _production = section.Production;

        foreach (var assemblyConfigurationElement in section.Assemblies)
        {
          _templateCompiler.AddReference(Assembly.Load(assemblyConfigurationElement.Name).Location);
        }

        foreach (var namespaceConfigurationElement in section.Namespaces)
        {
          _templateCompiler.AddUsing(namespaceConfigurationElement.Name);
        }
      }
    }

    public static void ClearViewCache()
    {
      lock (_viewCache)
      {
        _viewCache.Clear();
      }
    }

    public void RenderView(ViewContext viewContext)
    {
      var controller = (string)viewContext.RouteData.Values["controller"];
      var viewKey = controller + "/" + viewContext.ViewName;

      CompiledView compiledView;

      if (!_viewCache.TryGetValue(viewKey, out compiledView))
      {
        lock (_viewCache)
        {
          if (!_viewCache.TryGetValue(viewKey, out compiledView))
          {
            var templatePath = viewContext.HttpContext.Request
              .MapPath("~/Views/" + viewKey + ".haml");

            var layoutPath = FindLayout(viewContext.HttpContext.Request
              .MapPath("~/Views/Shared"), viewContext.MasterName, controller);

            compiledView = new CompiledView(_templateCompiler, templatePath, layoutPath, viewContext.ViewData);

            _viewCache.Add(viewKey, compiledView);
          }
        }
      }

      if (!_production)
      {
        compiledView.RecompileIfNecessary(viewContext.ViewData);
      }

      var view = compiledView.CreateView();

      view.Render(viewContext);
    }

    protected virtual string FindLayout(string mastersFolder, string masterName, string controller)
    {
      var layoutPath = mastersFolder + "\\" + masterName + ".haml";

      if (File.Exists(layoutPath))
      {
        return layoutPath;
      }

      layoutPath = mastersFolder + "\\" + controller + ".haml";

      if (File.Exists(layoutPath))
      {
        return layoutPath;
      }

      layoutPath = mastersFolder + "\\application.haml";

      if (File.Exists(layoutPath))
      {
        return layoutPath;
      }

      return null;
    }
  }
}