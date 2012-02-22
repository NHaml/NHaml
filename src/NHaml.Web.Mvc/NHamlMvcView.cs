using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Security.Permissions;
using System.Web;
using System.Web.Mvc;
using NHaml4.Crosscutting;
using NHaml4.TemplateBase;

namespace NHaml.Web.Mvc
{
    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public abstract class NHamlMvcView : NHamlMvcView<object>
    {
    };

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal )]
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
            Ajax = new AjaxHelper<TModel>( viewContext, this );
            Html = new HtmlHelper<TModel>( viewContext, this );
            Url = new UrlHelper( viewContext.RequestContext );
            //if (Master!=null) {
            //    if (Master is NHamlMvcView<TModel>)
            //    {
            //        ((NHamlMvcView<TModel>)Master).ViewContext = viewContext;
            //        ((NHamlMvcView<TModel>)Master).SetViewData(viewContext.ViewData);
            //        ((NHamlMvcView<TModel>)Master).CreateHelpers(viewContext);
            //    }
            //    else if (Master is NHamlMvcView<object>)
            //    {
            //        ((NHamlMvcView<object>)Master).ViewContext = viewContext;
            //        ((NHamlMvcView<object>)Master).SetViewData(viewContext.ViewData);
            //        ((NHamlMvcView<object>)Master).CreateHelpers(viewContext);
            //    }
            //}
        }

        public AjaxHelper<TModel> Ajax { get; protected set; }
        public HtmlHelper<TModel> Html { get; protected set; }
        public UrlHelper Url { get; protected set; }

        public ViewContext ViewContext { get; private set; }

        public new ViewDataDictionary<TModel> ViewData { get; private set; }

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