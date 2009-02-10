using System;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
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