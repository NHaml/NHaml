using System.Configuration;
using System.Security.Permissions;
using System.Web;

namespace NHaml.Configuration
{
  [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  public sealed class NamespaceConfigurationElement : KeyedConfigurationElement
  {
    private const string NamespaceElement = "namespace";

    public NamespaceConfigurationElement()
    {
    }

    public NamespaceConfigurationElement(string name)
    {
      Name = name;
    }

    public override string Key
    {
      get { return Name; }
    }

    [ConfigurationProperty(NamespaceElement, IsRequired = true, IsKey = true)]
    public string Name
    {
      get { return (string)this[NamespaceElement]; }
      set { this[NamespaceElement] = value; }
    }
  }
}