using System;
using System.Configuration;
using System.Globalization;
using System.Security.Permissions;
using System.Web;

using NHaml.BackEnds;
using NHaml.BackEnds.CSharp2;
using NHaml.BackEnds.CSharp3;
using NHaml.Properties;
using NHaml.Utils;

namespace NHaml.Configuration
{
  [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  public class NHamlSection : ConfigurationSection
  {
    private const string AssembliesElement = "assemblies";
    private const string CompilerBackEndAttribute = "compilerBackEnd";
    private const string NamespacesElement = "namespaces";
    private const string ProductionAttribute = "production";

    [ConfigurationProperty(ProductionAttribute)]
    public virtual bool Production
    {
      get { return Convert.ToBoolean(this[ProductionAttribute], CultureInfo.CurrentCulture); }
    }

    [ConfigurationProperty(CompilerBackEndAttribute)]
    public virtual string CompilerBackEnd
    {
      get { return Convert.ToString(this[CompilerBackEndAttribute], CultureInfo.CurrentCulture); }
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

    public ICompilerBackEnd CreateCompilerBackEnd()
    {
      var backEnd = CompilerBackEnd;

      var csharp2BackEndType = typeof(CSharp2CompilerBackEnd);
      var csharp3BackEndType = typeof(CSharp3CompilerBackEnd);

      Type backEndType;

      if (backEnd.IndexOf(Type.Delimiter) == -1)
      {
        if (!backEnd.EndsWith("CompilerBackEnd", StringComparison.OrdinalIgnoreCase))
        {
          backEnd += "CompilerBackEnd";
        }

        if (backEnd.Equals(csharp2BackEndType.Name, StringComparison.OrdinalIgnoreCase))
        {
          backEndType = csharp2BackEndType;
        }
        else
        {
          backEndType = backEnd.Equals(csharp3BackEndType.Name, StringComparison.OrdinalIgnoreCase)
            ? csharp3BackEndType
            : Type.GetType(backEnd, false);
        }
      }
      else
      {
        backEndType = Type.GetType(backEnd, false);
      }

      if (backEndType == null)
      {
        throw new ConfigurationErrorsException(
          Utility.FormatCurrentCulture(Resources.CompilerBackEndTypeNotFound, backEnd));
      }

      if (!typeof(ICompilerBackEnd).IsAssignableFrom(backEndType))
      {
        throw new ConfigurationErrorsException(
          Utility.FormatCurrentCulture(Resources.NotAssignableToICompilerBackEnd, backEnd));
      }

      return (ICompilerBackEnd)Activator.CreateInstance(backEndType);
    }
  }
}