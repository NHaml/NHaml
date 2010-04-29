using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Security.Permissions;
using System.Web;
using System.Web.Mvc;
using NHaml.Utils;

namespace NHaml.Web.Mvc
{
    [AspNetHostingPermission( SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal )]
    [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public abstract class NHamlMvcView<TModel> : Template, IView, IViewDataContainer
      where TModel : class
    {
        public void Render( ViewContext viewContext, TextWriter writer )
        {
            Invariant.ArgumentNotNull( viewContext, "viewContext" );

            ViewContext = viewContext;

            SetViewData( viewContext.ViewData );

            CreateHelpers( viewContext );

            Render( writer );
        }

        protected virtual void CreateHelpers( ViewContext viewContext )
        {
            Ajax = new AjaxHelper( viewContext, this );
            Html = new HtmlHelper( viewContext, this );
            Url = new UrlHelper( viewContext.RequestContext );
        }

        public AjaxHelper Ajax { get; protected set; }
        public HtmlHelper Html { get; protected set; }
        public UrlHelper Url { get; protected set; }

        public ViewContext ViewContext { get; private set; }

        public ViewDataDictionary<TModel> ViewData { get; private set; }

        public TempDataDictionary TempData
        {
            get { return ViewContext.TempData; }
        }

        public TModel Model
        {
            get { return ViewData.Model; }
        }

        [SuppressMessage( "Microsoft.Usage", "CA2227" )]
        [SuppressMessage( "Microsoft.Design", "CA1033" )]
        ViewDataDictionary IViewDataContainer.ViewData
        {
            get { return ViewData; }
            set { SetViewData( value ); }
        }

        private void SetViewData( ViewDataDictionary viewData )
        {
            if( typeof( ViewDataDictionary<TModel> ).IsAssignableFrom( viewData.GetType() ) )
            {
                ViewData = (ViewDataDictionary<TModel>)viewData;
            }
            else
            {
                ViewData = new ViewDataDictionary<TModel>( viewData );
            }
        }
    }
}