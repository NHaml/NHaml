using System;
using System.Configuration;
using System.Globalization;
using System.Security.Permissions;
using System.Web;

using NHaml.Backends;
using NHaml.Backends.CSharp2;
using NHaml.Backends.CSharp3;
using NHaml.Properties;
using NHaml.Utils;

namespace NHaml.Configuration
{
  [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  public class NHamlSection : ConfigurationSection
  {
    private const string AssembliesElement = "assemblies";
    private const string CompilerBackendAttribute = "compilerBackend";
    private const string NamespacesElement = "namespaces";
    private const string ProductionAttribute = "production";

    [ConfigurationProperty(ProductionAttribute)]
    public virtual bool Production
    {
      get { return Convert.ToBoolean(this[ProductionAttribute], CultureInfo.CurrentCulture); }
    }

    [ConfigurationProperty(CompilerBackendAttribute)]
    public virtual string CompilerBackend
    {
      get { return Convert.ToString(this[CompilerBackendAttribute], CultureInfo.CurrentCulture); }
    }

    [ConfigurationProperty(AssembliesElement)]
    public virtual AssembliesConfigurationCollection Assemblies
    {
      get { return (AssembliesConfigurationCollection)base[AssembliesElement]; }
    }

    [ConfigurationProperty(NamespacesElement)]
    public virtual NamespacesConfigurationCollection Namespaces
    {
      get { return (NamespacesConfigurationCollection)base[NamespacesElement]; }
    }

    public static NHamlSection Read()
    {
      return (NHamlSection)ConfigurationManager.GetSection("nhaml");
    }

    public ICompilerBackend CreateCompilerBackend()
    {
      var backend = CompilerBackend;

      var csharp2BackendType = typeof(CSharp2CompilerBackend);
      var csharp3BackendType = typeof(CSharp3CompilerBackend);
      Type backendType;

      if (backend.IndexOf(Type.Delimiter) == -1)
      {
        if (!backend.EndsWith("CompilerBackend"))
        {
          backend += "CompilerBackend";
        }

        if (backend.Equals(csharp2BackendType.Name, StringComparison.InvariantCulture))
        {
          backendType = csharp2BackendType;
        }
        else if (backend.Equals(csharp3BackendType.Name, StringComparison.InvariantCulture))
        {
          backendType = csharp3BackendType;
        }
        else
        {
          backendType = Type.GetType(backend, false);
        }
      }
      else
      {
        backendType = Type.GetType(backend, false);
      }

      if (backendType == null)
      {
        throw new ConfigurationErrorsException(
          Utility.FormatCurrentCulture(Resources.CompilerBackendTypeNotFound, backend));
      }

      if (!typeof(ICompilerBackend).IsAssignableFrom(backendType))
      {
        throw new ConfigurationErrorsException(
          Utility.FormatCurrentCulture(Resources.NotAssignableToICompilerBackend, backend));
      }

      return (ICompilerBackend)Activator.CreateInstance(backendType);
    }
  }
}