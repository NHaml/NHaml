using System;
using System.Data.Linq;
using System.Linq.Expressions;
using System.Security.Permissions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using System.Web.UI;

namespace NHaml.Web.Mvc
{
  [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  public class NHamlMvcViewEngine : VirtualPathProviderViewEngine
  {
    private readonly TemplateEngine _templateEngine = new TemplateEngine();

    public virtual string DefaultMaster { get; set; }

    public NHamlMvcViewEngine()
    {
      InitializeTemplateEngine();
      InitializeViewLocations();
    }

    private void InitializeTemplateEngine()
    {
      DefaultMaster = "application";

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
    }

    protected TemplateEngine TemplateEngine
    {
      get { return _templateEngine; }
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
        "~/Views/Shared/{0}.haml"
      };

      PartialViewLocationFormats = ViewLocationFormats;
    }

    public override ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName)
    {
      if (string.IsNullOrEmpty(masterName))
      {
        var controllerName = controllerContext.RouteData.GetRequiredString("controller");
        var result = base.FindView(controllerContext, viewName, controllerName);

        if (result.View == null)
        {
          result = base.FindView(controllerContext, viewName, DefaultMaster);
        }

        return result.View == null ? base.FindPartialView(controllerContext, viewName) : result;
      }

      return base.FindView(controllerContext, viewName, masterName);
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

    protected virtual Type ViewGenericBaseType
    {
      get { return typeof(NHamlMvcView<>); }
    }

    protected virtual Type GetViewBaseType(ControllerContext controllerContext)
    {
      var modelType = typeof(object);

      var viewData = controllerContext.Controller.ViewData;

      var viewContext = controllerContext as ViewContext;

      if ((viewContext != null) && (viewContext.ViewData != null))
      {
        viewData = viewContext.ViewData;
      }

      if ((viewData != null) && (viewData.Model != null))
      {
        modelType = viewData.Model.GetType();
      }

      return ViewGenericBaseType.MakeGenericType(modelType);
    }

    protected virtual string VirtualPathToPhysicalPath(RequestContext context, string path)
    {
      return context.HttpContext.Request.MapPath(path);
    }
  }
}