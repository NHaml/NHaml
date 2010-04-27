using System;
using System.Security.Permissions;
using System.Web;

namespace NHaml.Web.Mvc.IronRuby
{
    [AspNetHostingPermission( SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal )]
    [AspNetHostingPermission( SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal )]
    public class NHamlMvcIronRubyViewEngine : NHamlMvcViewEngine
    {
        public NHamlMvcIronRubyViewEngine()
        {
            TemplateEngine.Options.TemplateCompiler = new NHamlMvcIronRubyTemplateCompiler();

            TemplateEngine.Options.AddReference( typeof( NHamlMvcIronRubyView<> ).Assembly.Location );
        }

        protected override Type ViewGenericBaseType
        {
            get { return typeof( NHamlMvcIronRubyView<> ); }
        }
    }
}