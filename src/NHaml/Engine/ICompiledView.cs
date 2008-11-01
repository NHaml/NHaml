using System.Security.Permissions;
using System.Web;

namespace NHaml.Engine
{
  [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  public interface ICompiledView<TView>
  {
    TView CreateView();
    void RecompileIfNecessary();
  }
}