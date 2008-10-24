using System.Security.Permissions;
using System.Web;
using System.Web.Mvc;

namespace NHaml.Web.Mvc
{
  [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  public interface INHamlMvcView : IViewDataContainer, IView
  {
    void Render(ViewContext viewContext);
  }
}