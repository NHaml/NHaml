using System;
using System.Configuration;
using System.Globalization;
using System.Security.Permissions;
using System.Web;

namespace NHaml.Configuration
{
  [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  public sealed class NHamlSection : ConfigurationSection
  {
    private const string ProductionAttribute = "production";
    private const string AssembliesElement = "assemblies";
    private const string NamespacesElement = "namespaces";

    public static NHamlSection Read()
    {
      return (NHamlSection)ConfigurationManager.GetSection("nhaml");
    }

    [ConfigurationProperty(ProductionAttribute)]
    public bool Production
    {
      get { return Convert.ToBoolean(this[ProductionAttribute], CultureInfo.CurrentCulture); }
    }

    [ConfigurationProperty(AssembliesElement)]
    public AssembliesConfigurationCollection Assemblies
    {
      get { return (AssembliesConfigurationCollection)base[AssembliesElement]; }
    }

    [ConfigurationProperty(NamespacesElement)]
    public NamespacesConfigurationCollection Namespaces
    {
      get { return (NamespacesConfigurationCollection)base[NamespacesElement]; }
    }
  }
}