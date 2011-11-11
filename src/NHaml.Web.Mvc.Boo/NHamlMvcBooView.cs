using System.Security.Permissions;
using System.Web;
using System.Web.Mvc;

using NHaml.Web.Mvc.Boo.Helpers;

using AjaxHelper = System.Web.Mvc.AjaxHelper;

namespace NHaml.Web.Mvc.Boo
{
    [AspNetHostingPermission( SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal )]
    [AspNetHostingPermission( SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal )]
    public abstract class NHamlMvcBooView<TModel> : NHamlMvcView<TModel>
      where TModel : class
    {
        protected override void CreateHelpers( ViewContext viewContext )
        {
            Ajax = new AjaxHelper<TModel>( viewContext, this );
            Html = new HtmlHelper<TModel>( viewContext, this );
            Url = new BooUrlHelper( viewContext.RequestContext );
        }

        // we need to extend the UrlHelper instead of 
        // creating extension methods bececause the Boo
        // and CSharp compiler search for the first
        // matching method, and if it finds it in the class
        // it never searchs for an extension method.
        public new BooUrlHelper Url { get; set; }
    }
}
