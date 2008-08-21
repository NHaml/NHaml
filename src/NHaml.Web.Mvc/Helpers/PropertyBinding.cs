using System.Security.Permissions;
using System.Web;

namespace NHaml.Web.Mvc
{
  [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  public class PropertyBinding
  {
    public string Name { get; set; }
    public string BindingName { get; set; }
    public string Value { get; set; }
  }
}