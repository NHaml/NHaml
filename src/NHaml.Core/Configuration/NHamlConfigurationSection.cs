using System;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Security.Permissions;
using System.Web;
using NHaml.Core.Utils;
using NHaml.Core.Template;
using NHaml.Core.Compilers;

namespace NHaml.Core.Configuration
{
	[AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
	[AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
	public class NHamlConfigurationSection : ConfigurationSection
	{
		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
		public static NHamlConfigurationSection GetSection()
		{
			return (NHamlConfigurationSection)ConfigurationManager.GetSection("nhaml");
		}

		public static void UpdateTemplateOptions(TemplateOptions options)
		{
			if (options == null)
				throw new ArgumentNullException("options");

			var section = GetSection();

			if (section == null)
			{
				return;
			}

			if (section.AutoRecompile.HasValue)
			{
				options.AutoRecompile = section.AutoRecompile.Value;
			}
			else
			{
				options.AutoRecompile = true;
			}

			if (!string.IsNullOrEmpty(section.TemplateBaseType))
			{
				options.TemplateBaseType = Type.GetType(section.TemplateBaseType, true, false);
			}

			if (section.EncodeHtml.HasValue)
			{
				options.EncodeHtml = section.EncodeHtml.Value;
			}

			if (section.OutputDebugFiles.HasValue)
			{
				options.OutputDebugFiles = section.OutputDebugFiles.Value;
			}

			if (!string.IsNullOrEmpty(section.TemplateCompiler))
			{
				options.TemplateCompiler = section.CreateTemplateCompiler();
			}

			foreach (var assemblyConfigurationElement in section.Assemblies)
			{
				Assembly assembly;
				var assemblyName = assemblyConfigurationElement.Name;
				try
				{
					assembly = Assembly.Load(assemblyName);
                    options.AddReference(assembly.Location);
				}
				catch (Exception exception)
				{
					var message = string.Format("Coule not load Assembly '{0}'.Did you forget to fully qualify it?eg 'System.Xml, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'", assemblyName);
					throw new Exception(message, exception);
				}
			}

			foreach (var namespaceConfigurationElement in section.Namespaces)
			{
				options.AddUsing(namespaceConfigurationElement.Name);
			}
		}

		private const string AssembliesElement = "assemblies";
		private const string NamespacesElement = "namespaces";
		private const string AutoRecompileAttribute = "autoRecompile";
		private const string EncodeHtmlAttribute = "encodeHtml";
		private const string OutputDebugFilesAttribute = "outputDebugFiles";
		private const string TemplateCompilerAttribute = "templateCompiler";
		private const string TemplateBaseTypeAttribute = "templateBaseType";

		[ConfigurationProperty(AutoRecompileAttribute)]
		public virtual bool? AutoRecompile
		{
			get { return this[AutoRecompileAttribute] as bool?; }
		}

		[ConfigurationProperty(EncodeHtmlAttribute)]
		public virtual bool? EncodeHtml
		{
			get { return this[EncodeHtmlAttribute] as bool?; }
		}
		[ConfigurationProperty(OutputDebugFilesAttribute)]
		public virtual bool? OutputDebugFiles
		{
			get { return this[OutputDebugFilesAttribute] as bool?; }
		}

		[ConfigurationProperty(TemplateCompilerAttribute)]
		public virtual string TemplateCompiler
		{
			get { return Convert.ToString(this[TemplateCompilerAttribute], CultureInfo.CurrentCulture); }
		}
		[ConfigurationProperty(TemplateBaseTypeAttribute)]
		public virtual string TemplateBaseType
		{
			get { return Convert.ToString(this[TemplateBaseTypeAttribute], CultureInfo.CurrentCulture); }
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

		public IClassBuilder CreateTemplateCompiler()
		{
			var templateCompiler = TemplateCompiler;

			Type type;

			if (templateCompiler.IndexOf(Type.Delimiter) == -1)
			{
				if (!templateCompiler.EndsWith("TemplateCompiler", StringComparison.OrdinalIgnoreCase))
				{
					templateCompiler += "TemplateCompiler";
				}
				type = Type.GetType(templateCompiler, false);
			}
			else
			{
				type = Type.GetType(templateCompiler, false);
			}

			if (type == null)
			{
				var message = Utility.FormatCurrentCulture("TemplateCompiler type '{0}' not found", templateCompiler);
				throw new ConfigurationErrorsException(message);
			}

			if (!typeof(IClassBuilder).IsAssignableFrom(type))
			{
				var message = Utility.FormatCurrentCulture("Type '{0}' is not assignable to ITemplateCompiler", templateCompiler);
				throw new ConfigurationErrorsException(message);
			}

			return (IClassBuilder)Activator.CreateInstance(type);
		}
	}
}