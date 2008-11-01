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

using NHaml.Engine;

namespace NHaml.Web.Mvc
{
  [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  public class NHamlMvcViewEngine : VirtualPathProviderViewEngine
  {
    private readonly CompiledViewCache<INHamlMvcView> _viewCache =
      new CompiledViewCache<INHamlMvcView>();

    public NHamlMvcViewEngine()
    {
      InitializeTemplateCompiler();
      InitializeViewLocations();
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

    private void InitializeTemplateCompiler()
    {
      TemplateCompiler.AddUsing("System.Web");
      TemplateCompiler.AddUsing("System.Web.Mvc");
      TemplateCompiler.AddUsing("System.Web.Mvc.Html");
      TemplateCompiler.AddUsing("System.Web.Routing");

      TemplateCompiler.AddUsing("NHaml.Web.Mvc");

      TemplateCompiler.ViewBaseType = typeof(NHamlMvcView<>);

      TemplateCompiler.AddReference(typeof(UserControl).Assembly.Location);
      TemplateCompiler.AddReference(typeof(RouteValueDictionary).Assembly.Location);
      TemplateCompiler.AddReference(typeof(DataContext).Assembly.Location);
      TemplateCompiler.AddReference(typeof(LinkExtensions).Assembly.Location);
      TemplateCompiler.AddReference(typeof(Action).Assembly.Location);
      TemplateCompiler.AddReference(typeof(Expression).Assembly.Location);
      TemplateCompiler.AddReference(typeof(IView).Assembly.Location);

      foreach (var name in Assembly.GetCallingAssembly().GetReferencedAssemblies())
      {
        TemplateCompiler.AddReference(Assembly.Load(name).Location);
      }

      TemplateCompiler.LoadFromConfiguration();
    }

    protected TemplateCompiler TemplateCompiler
    {
      get { return _viewCache.TemplateCompiler; }
    }

    public override ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName)
    {
      masterName = MasterRenamingHackery(masterName);

      var result = base.FindView(controllerContext, viewName, masterName);

      return result;
    }

    private static string MasterRenamingHackery(string masterName)
    {
      //Hack: If the specified master is empty then the VirtualPathProviderViewEngine will not try to locate a master.
      //Give it a fake name to ensure that the VirtualPathProviderViewEngine will always look through the master-locations 
      //so that application.haml can be located

      if (string.IsNullOrEmpty(masterName))
      {
        masterName = "__NhamlFakeMasterThatShouldNeverExist__";
      }

      return masterName;
    }

    protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
    {
      return _viewCache.GetView(
        () => CreateCompiledView(partialPath, null, controllerContext),
        partialPath,
        GetBaseType(controllerContext));
    }

    protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
    {
      return _viewCache.GetView(
        () => CreateCompiledView(viewPath, masterPath, controllerContext),
        viewPath,
        GetBaseType(controllerContext));
    }

    private static Type GetBaseType(ControllerContext viewContext)
    {
      var modelType = typeof(object);
      var viewData = viewContext.Controller.ViewData;
      if ((viewData != null) && (viewData.Model != null))
      {
        modelType = viewData.Model.GetType();
      }

      return typeof(NHamlMvcView<>).MakeGenericType(modelType);
    }

    protected CompiledView<INHamlMvcView> CreateCompiledView(string viewPath, string masterPath, RequestContext context)
    {
      viewPath = VirtualPathToPhysicalPath(viewPath, context);
      masterPath = VirtualPathToPhysicalPath(masterPath, context);

      return new CompiledView<INHamlMvcView>(
        TemplateCompiler,
        viewPath,
        masterPath);
    }

    private static string VirtualPathToPhysicalPath(string path, RequestContext context)
    {
      if (string.IsNullOrEmpty(path))
      {
        return path;
      }

      return context.HttpContext.Request.MapPath(path);
    }
  }
}