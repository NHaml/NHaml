using System.Diagnostics.CodeAnalysis;
using System.Security.Permissions;
using System.Web;

namespace NHaml.Configuration
{
  [SuppressMessage("Microsoft.Design", "CA1010")]
  [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  public sealed class NamespacesConfigurationCollection : ConfigurationCollection<NamespaceConfigurationElement>
  {
  }
}