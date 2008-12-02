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
      TemplateEngine.TemplateCompiler = new BooTemplateCompiler();

      TemplateEngine.AddReference( typeof( NHamlMvcBooViewEngine ).Assembly );

      // since boo currently only support clr extensions when
      // they are directly imported, we add them here
      TemplateEngine.AddUsing( typeof( FormExtensions ).FullName );
      TemplateEngine.AddUsing( typeof( InputExtensions ).FullName );
      TemplateEngine.AddUsing( typeof( LinkExtensions ).FullName );
      TemplateEngine.AddUsing( typeof( RenderPartialExtensions ).FullName );
      TemplateEngine.AddUsing( typeof( MvcForm ).FullName );
      TemplateEngine.AddUsing( typeof( SelectExtensions ).FullName );
      TemplateEngine.AddUsing( typeof( TextAreaExtensions ).FullName );
      TemplateEngine.AddUsing( typeof( ValidationExtensions ).FullName );

      TemplateEngine.AddUsing( typeof( BooFormExtensions ).FullName );
      TemplateEngine.AddUsing( typeof( BooInputExtensions ).FullName );
      TemplateEngine.AddUsing( typeof( BooLinkExtensions ).FullName );
      TemplateEngine.AddUsing( typeof( BooSelectExtensions ).FullName );
      TemplateEngine.AddUsing( typeof( BooTextAreaExtensions ).FullName );
      TemplateEngine.AddUsing( typeof( BooValidationExtensions ).FullName );
    }

    protected override Type ViewGenericBaseType
    {
      get { return typeof( NHamlMvcBooView<> ); }
    }

  }
}
