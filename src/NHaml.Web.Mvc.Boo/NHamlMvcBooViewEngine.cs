using System;
using System.Security.Permissions;
using System.Web;
using System.Web.Mvc.Html;

using NHaml.Compilers.Boo;
using NHaml.Web.Mvc.Boo.Extensions;

namespace NHaml.Web.Mvc.Boo
{
    [AspNetHostingPermission( SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal )]
    [AspNetHostingPermission( SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal )]
    public class NHamlMvcBooViewEngine : NHamlMvcViewEngine
    {
        public NHamlMvcBooViewEngine()
        {
            TemplateEngine.Options.TemplateCompiler = new BooTemplateCompiler();

            TemplateEngine.Options.AddReference( typeof( NHamlMvcBooViewEngine ).Assembly );

            // since boo currently only support clr extensions when
            // they are directly imported, we add them here
            TemplateEngine.Options.AddUsing( typeof( FormExtensions ).FullName );
            TemplateEngine.Options.AddUsing( typeof( InputExtensions ).FullName );
            TemplateEngine.Options.AddUsing( typeof( LinkExtensions ).FullName );
            TemplateEngine.Options.AddUsing( typeof( RenderPartialExtensions ).FullName );
            TemplateEngine.Options.AddUsing( typeof( MvcForm ).FullName );
            TemplateEngine.Options.AddUsing( typeof( SelectExtensions ).FullName );
            TemplateEngine.Options.AddUsing( typeof( TextAreaExtensions ).FullName );
            TemplateEngine.Options.AddUsing( typeof( ValidationExtensions ).FullName );

            TemplateEngine.Options.AddUsing( typeof( BooFormExtensions ).FullName );
            TemplateEngine.Options.AddUsing( typeof( BooInputExtensions ).FullName );
            TemplateEngine.Options.AddUsing( typeof( BooLinkExtensions ).FullName );
            TemplateEngine.Options.AddUsing( typeof( BooSelectExtensions ).FullName );
            TemplateEngine.Options.AddUsing( typeof( BooTextAreaExtensions ).FullName );
            TemplateEngine.Options.AddUsing( typeof( BooValidationExtensions ).FullName );
        }

        protected override Type ViewGenericBaseType
        {
            get { return typeof( NHamlMvcBooView<> ); }
        }

    }
}
