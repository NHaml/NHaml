using System.Security.Permissions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace NHaml.Web.Mvc
{
  [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  public class NHamlControllerFactory : DefaultControllerFactory
  {
    protected override IController CreateController(RequestContext requestContext, string controllerName)
    {
      var controller = base.CreateController(requestContext, controllerName);

      var c = controller as Controller;

      if (c != null)
      {
        c.ViewEngine = new NHamlViewEngine();
      }

      return controller;
    }
  }
}