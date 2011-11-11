using System.Security.Permissions;
using System.Web;
using System.Web.Mvc;

using NHaml.Web.Mvc.IronRuby.Helpers;

namespace NHaml.Web.Mvc.IronRuby
{
    [AspNetHostingPermission( SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal )]
    [AspNetHostingPermission( SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal )]
    public abstract class NHamlMvcIronRubyView<TModel> : NHamlMvcView<TModel>
      where TModel : class
    {
        protected override void CreateHelpers( ViewContext viewContext )
        {
            Ajax = new AjaxHelper<TModel>( viewContext, this );
            Html = new NHamlMvcIronRubyHtmlHelper<TModel>( viewContext, this );
            Url = new NHamlMvcIronRubyUrlHelper( viewContext.RequestContext );
        }
    }
}