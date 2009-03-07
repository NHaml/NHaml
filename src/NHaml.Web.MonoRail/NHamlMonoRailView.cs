using System.Collections;
using System.IO;
using System.Security.Permissions;
using System.Web;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Framework.Helpers;
using NHaml.Utils;

namespace NHaml.Web.MonoRail
{
    [AspNetHostingPermission( SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal )]
    [AspNetHostingPermission( SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal )]
    public abstract class NHamlMonoRailView : Template
    {
        public void Render(IEngineContext engineContext, TextWriter writer, IControllerContext controllerContext)
        {
            Invariant.ArgumentNotNull(controllerContext, "controllerContext");
            Invariant.ArgumentNotNull(engineContext, "engineContext");

            ViewContext = engineContext;

            ViewData = controllerContext.PropertyBag;

            CreateHelpers(engineContext);

            Render(writer);
        }

        

        protected virtual void CreateHelpers(IEngineContext engineContext)
        {
            Ajax = new AjaxHelper(engineContext);
            Html = new HtmlHelper(engineContext);
            Url = new UrlHelper(engineContext);
        }

        public AjaxHelper Ajax { get; protected set; }
        public HtmlHelper Html { get; protected set; }
        public UrlHelper Url { get; protected set; }

        public IEngineContext ViewContext { get; private set; }

        public IDictionary ViewData { get; private set; }


    }
}