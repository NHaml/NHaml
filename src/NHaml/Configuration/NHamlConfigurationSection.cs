using System;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Security.Permissions;
using System.Web;

using NHaml.Compilers;
using NHaml.Compilers.CSharp2;
using NHaml.Compilers.CSharp3;
using NHaml.Properties;
using NHaml.Utils;

namespace NHaml.Configuration
{
    [AspNetHostingPermission( SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal )]
    [AspNetHostingPermission( SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal )]
    public class NHamlConfigurationSection : ConfigurationSection
    {
        [SuppressMessage( "Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate" )]
        public static NHamlConfigurationSection GetSection()
        {
            return (NHamlConfigurationSection)ConfigurationManager.GetSection( "nhaml" );
        }

        public static void UpdateTemplateOptions( TemplateOptions options )
        {
            if( options == null )
                throw new ArgumentNullException( "options" );

            var section = GetSection();

            if( section == null )
                return;

            if (section.IndentSize.HasValue)
                options.IndentSize = section.IndentSize.Value;

            if( section.AutoRecompile.HasValue )
                options.AutoRecompile = section.AutoRecompile.Value;

            if( section.UseTabs.HasValue )
                options.UseTabs = section.UseTabs.Value;

            if( section.EncodeHtml.HasValue )
                options.EncodeHtml = section.EncodeHtml.Value;

            if( !string.IsNullOrEmpty( section.TemplateCompiler ) )
                options.TemplateCompiler = section.CreateTemplateCompiler();

                foreach( var assemblyConfigurationElement in section.Assemblies )
                    options.AddReference( Assembly.Load( assemblyConfigurationElement.Name ).Location );

            foreach( var namespaceConfigurationElement in section.Namespaces )
                options.AddUsing( namespaceConfigurationElement.Name );
        }

        private const string AssembliesElement = "assemblies";
        private const string NamespacesElement = "namespaces";
        private const string AutoRecompileAttribute = "autoRecompile";
        private const string EncodeHtmlAttribute = "encodeHtml";
        private const string TemplateCompilerAttribute = "templateCompiler";
        private const string UseTabsAttribute = "useTabs";
        private const string IndentSizeAttribute = "indentSize";

        [ConfigurationProperty( AutoRecompileAttribute )]
        public virtual bool? AutoRecompile
        {
            get { return this[AutoRecompileAttribute] as bool?; }
        }

        [ConfigurationProperty( EncodeHtmlAttribute )]
        public virtual bool? EncodeHtml
        {
            get { return this[EncodeHtmlAttribute] as bool?; }
        }

        [ConfigurationProperty( UseTabsAttribute )]
        public virtual bool? UseTabs
        {
            get { return this[UseTabsAttribute] as bool?; }
        }

        [ConfigurationProperty( IndentSizeAttribute )]
        public virtual int? IndentSize
        {
            get { return this[IndentSizeAttribute] as int?; }
        }

        [ConfigurationProperty( TemplateCompilerAttribute )]
        public virtual string TemplateCompiler
        {
            get { return Convert.ToString( this[TemplateCompilerAttribute], CultureInfo.CurrentCulture ); }
        }

        [ConfigurationProperty( AssembliesElement )]
        public virtual AssembliesConfigurationCollection Assemblies
        {
            get { return (AssembliesConfigurationCollection)base[AssembliesElement]; }
        }

        [ConfigurationProperty( NamespacesElement )]
        public virtual NamespacesConfigurationCollection Namespaces
        {
            get { return (NamespacesConfigurationCollection)base[NamespacesElement]; }
        }

        public ITemplateCompiler CreateTemplateCompiler()
        {
            var templateCompiler = TemplateCompiler;

            var csharp2Type = typeof( CSharp2TemplateCompiler );
            var csharp3Type = typeof( CSharp3TemplateCompiler );

            Type Type;

            if( templateCompiler.IndexOf( Type.Delimiter ) == -1 )
            {
                if( !templateCompiler.EndsWith( "TemplateCompiler", StringComparison.OrdinalIgnoreCase ) )
                {
                    templateCompiler += "TemplateCompiler";
                }

                if( templateCompiler.Equals( csharp2Type.Name, StringComparison.OrdinalIgnoreCase ) )
                {
                    Type = csharp2Type;
                }
                else
                {
                    Type = templateCompiler.Equals( csharp3Type.Name, StringComparison.OrdinalIgnoreCase )
                      ? csharp3Type
                      : Type.GetType( templateCompiler, false );
                }
            }
            else
            {
                Type = Type.GetType( templateCompiler, false );
            }

            if( Type == null )
            {
                throw new ConfigurationErrorsException(
                  Utility.FormatCurrentCulture( Resources.TemplateCompilerTypeNotFound, templateCompiler ) );
            }

            if( !typeof( ITemplateCompiler ).IsAssignableFrom( Type ) )
            {
                throw new ConfigurationErrorsException(
                  Utility.FormatCurrentCulture( Resources.NotAssignableToITemplateCompiler, templateCompiler ) );
            }

            return (ITemplateCompiler)Activator.CreateInstance( Type );
        }
    }
}