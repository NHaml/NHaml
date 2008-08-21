using System.Data.Linq;
using System.IO;
using System.Linq.Expressions;
using System.Security.Permissions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI;

using NHaml.Engine;

namespace NHaml.Web.Mvc
{
  [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  public class NHamlViewEngine : ViewEngine<MvcCompiledView, ViewContext, IMvcView, ViewDataDictionary>, IViewEngine
  {
    private static readonly IViewEngine _instance = new NHamlViewEngine();

    public static IViewEngine Instance
    {
      get { return _instance; }
    }

    private NHamlViewEngine()
    {
      TemplateCompiler.AddUsing("System.Web");
      TemplateCompiler.AddUsing("System.Web.Mvc");
      TemplateCompiler.AddUsing("System.Web.Routing");

      TemplateCompiler.AddUsing("NHaml.Web.Mvc");

      TemplateCompiler.ViewBaseType = typeof(MvcView<>);

      TemplateCompiler.AddReference(typeof(UserControl).Assembly.Location);
      TemplateCompiler.AddReference(typeof(RouteValueDictionary).Assembly.Location);
      TemplateCompiler.AddReference(typeof(DataContext).Assembly.Location);
      TemplateCompiler.AddReference(typeof(TextInputExtensions).Assembly.Location);
      TemplateCompiler.AddReference(typeof(Expression).Assembly.Location);

      TemplateCompiler.LoadFromConfiguration();
    }

    protected override MvcCompiledView CreateView(ViewContext viewContext)
    {
      var templatePath = viewContext.HttpContext.Request
        .MapPath("~/Views/" + GetViewKey(viewContext) + ".haml");

      var layoutPath = SelectLayout(viewContext);

      return new MvcCompiledView(
        TemplateCompiler,
        templatePath,
        layoutPath,
        viewContext.ViewData);
    }

    protected override void RenderView(IMvcView view, ViewContext viewContext)
    {
      view.Render(viewContext);
    }

    protected override string GetViewKey(ViewContext viewContext)
    {
      return GetControllerName(viewContext) + "/" + viewContext.ViewName;
    }

    protected override ViewDataDictionary GetViewData(ViewContext viewContext)
    {
      return viewContext.ViewData;
    }

    protected override string SelectLayout(ViewContext viewContext)
    {
      var layoutsFolder = viewContext.HttpContext.Request.MapPath("~/Views/Shared");

      var layoutPath = layoutsFolder + "\\" + viewContext.MasterName + ".haml";

      if (File.Exists(layoutPath))
      {
        return layoutPath;
      }

      layoutPath = layoutsFolder + "\\" + GetControllerName(viewContext) + ".haml";

      if (File.Exists(layoutPath))
      {
        return layoutPath;
      }

      layoutPath = layoutsFolder + "\\application.haml";

      if (File.Exists(layoutPath))
      {
        return layoutPath;
      }

      return null;
    }

    private static string GetControllerName(RequestContext viewContext)
    {
      return (string)viewContext.RouteData.Values["controller"];
    }
  }
}