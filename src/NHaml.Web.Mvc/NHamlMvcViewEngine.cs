using System;
using System.Data.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Permissions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI;

using Microsoft.Web.Mvc;

namespace NHaml.Web.Mvc
{
  [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  public class NHamlMvcViewEngine : VirtualPathProviderViewEngine
  {
    private readonly TemplateEngine _templateEngine = new TemplateEngine();

    public NHamlMvcViewEngine()
    {
      InitializeTemplateEngine();
      InitializeViewLocations();
    }

    protected TemplateEngine TemplateEngine
    {
      get { return _templateEngine; }
    }

    private void InitializeTemplateEngine()
    {
      _templateEngine.AddUsing("System.Web");
      _templateEngine.AddUsing("System.Web.Mvc");
      _templateEngine.AddUsing("System.Web.Mvc.Html");
      _templateEngine.AddUsing("System.Web.Routing");

      _templateEngine.AddUsing("NHaml.Web.Mvc");

      _templateEngine.AddReference(typeof(UserControl).Assembly.Location);
      _templateEngine.AddReference(typeof(RouteValueDictionary).Assembly.Location);
      _templateEngine.AddReference(typeof(DataContext).Assembly.Location);
      _templateEngine.AddReference(typeof(LinkExtensions).Assembly.Location);
      _templateEngine.AddReference(typeof(Action).Assembly.Location);
      _templateEngine.AddReference(typeof(Expression).Assembly.Location);
      _templateEngine.AddReference(typeof(IView).Assembly.Location);
      _templateEngine.AddReference(typeof(NHamlMvcView<>).Assembly.Location);

      foreach (var name in Assembly.GetCallingAssembly().GetReferencedAssemblies())
      {
        _templateEngine.AddReference(Assembly.Load(name).Location);
      }
    }

    private void InitializeViewLocations()
    {
      ViewLocationFormats = new[]
                              {
                                "~/Views/{1}/{0}.haml",
                                "~/Views/Shared/{0}.haml"
                              };

      MasterLocationFormats = new[]
                                {
                                  "~/Views/Shared/{0}.haml",
                                  "~/Views/Shared/{1}.haml",
                                  "~/Views/Shared/application.haml",
                                };

      PartialViewLocationFormats = ViewLocationFormats;
    }

    public override ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName)
    {
      if (string.IsNullOrEmpty(masterName))
      {
        // Hack: If the specified master is empty then the VirtualPathProviderViewEngine will not try to locate a master.
        // Give it a fake name to ensure that the VirtualPathProviderViewEngine will always look through the master-locations 
        // so that application.haml can be located

        masterName = "__NHamlFakeMasterThatShouldNeverExist__";
      }

      var result = base.FindView(controllerContext, viewName, masterName);

      return result;
    }

    protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
    {
      return (IView)_templateEngine.Compile(
        VirtualPathToPhysicalPath(controllerContext, partialPath),
        GetViewBaseType(controllerContext)).CreateInstance();
    }

    protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
    {
      return (IView)_templateEngine.Compile(
        VirtualPathToPhysicalPath(controllerContext, viewPath),
        VirtualPathToPhysicalPath(controllerContext, masterPath),
        GetViewBaseType(controllerContext)).CreateInstance();
    }

    private Type GetViewBaseType(ControllerContext controllerContext)
    {
      var modelType = typeof(object);

      var viewData = controllerContext.Controller.ViewData;

      if ((viewData != null) && (viewData.Model != null))
      {
        modelType = viewData.Model.GetType();
      }

      return ViewBaseType.MakeGenericType(modelType);
    }

    protected virtual Type ViewBaseType
    {
      get { return typeof(NHamlMvcView<>); }
    }

    private static string VirtualPathToPhysicalPath(RequestContext context, string path)
    {
      return context.HttpContext.Request.MapPath(path);
    }
  }
}