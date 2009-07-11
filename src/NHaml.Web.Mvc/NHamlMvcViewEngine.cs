using System;
using System.Data.Linq;
using System.Linq.Expressions;

#if (NET4)
using System.Reflection;
using System.Security;
#endif
using System.Security.Permissions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using System.Web.UI;

namespace NHaml.Web.Mvc
{
    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class NHamlMvcViewEngine : VirtualPathProviderViewEngine
    {
        private readonly TemplateEngine _templateEngine = new TemplateEngine();


        public string DefaultMaster { get; set; }

#if NET4
        [SecuritySafeCritical]
        [SecurityCritical]
#endif
        public NHamlMvcViewEngine()
        {
            InitializeTemplateEngine();
            InitializeViewLocations();
        }

#if NET4
        [SecuritySafeCritical]
        [SecurityCritical]
#endif
        private void InitializeTemplateEngine()
        {
            DefaultMaster = "Application";

            _templateEngine.Options.AddUsing( "System.Web" );
            _templateEngine.Options.AddUsing( "System.Web.Mvc" );
            _templateEngine.Options.AddUsing( "System.Web.Mvc.Html" );
            _templateEngine.Options.AddUsing( "System.Web.Routing" );

            _templateEngine.Options.AddUsing( "NHaml.Web.Mvc" );

#if NET4
            _templateEngine.Options.AddReference( Assembly.Load("System.Web.Routing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35").Location );
#endif
            _templateEngine.Options.AddReference( typeof( UserControl ).Assembly.Location );
            _templateEngine.Options.AddReference( typeof( RouteValueDictionary ).Assembly.Location );
            _templateEngine.Options.AddReference( typeof( DataContext ).Assembly.Location );
            _templateEngine.Options.AddReference( typeof( LinkExtensions ).Assembly.Location );
            _templateEngine.Options.AddReference( typeof( Action ).Assembly.Location );
            _templateEngine.Options.AddReference( typeof( Expression ).Assembly.Location );
            _templateEngine.Options.AddReference( typeof( IView ).Assembly.Location );
            _templateEngine.Options.AddReference( typeof( NHamlMvcView<> ).Assembly.Location );
        }

        protected TemplateEngine TemplateEngine
        {
            get { return _templateEngine; }
        }

        private void InitializeViewLocations()
        {
            ViewLocationFormats = new[]
            {
                "~/Views/{1}/{0}.haml",
                "~/Views/Shared/{0}.haml"
            };

            MasterLocationFormats = new[]
            {
                "~/Views/Shared/{0}.haml"
            };

            PartialViewLocationFormats = ViewLocationFormats;
        }

#if NET4
        [SecuritySafeCritical]
        [SecurityCritical]
#endif
        public override ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            if (string.IsNullOrEmpty(masterName))
            {
                var controllerName = controllerContext.RouteData.GetRequiredString("controller");
                var result = base.FindView(controllerContext, viewName, controllerName, useCache);

                if (result.View == null)
                {
                    result = base.FindView(controllerContext, viewName, DefaultMaster, useCache);
                }

                return result.View == null ? base.FindPartialView(controllerContext, viewName, useCache) : result;
            }

            return base.FindView(controllerContext, viewName, masterName, useCache);
        }

#if NET4
        [SecuritySafeCritical]
        [SecurityCritical]
#endif
        protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
        {
            return (IView)_templateEngine.Compile(
                VirtualPathToPhysicalPath(controllerContext.RequestContext, partialPath),
                GetViewBaseType(controllerContext)).CreateInstance();
        }

#if NET4
        [SecuritySafeCritical]
        [SecurityCritical]
#endif
        protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
        {
            return (IView)_templateEngine.Compile(
                VirtualPathToPhysicalPath(controllerContext.RequestContext, viewPath),
                VirtualPathToPhysicalPath(controllerContext.RequestContext, masterPath),
                GetViewBaseType(controllerContext)).CreateInstance();
        }

//#if NET4
//        [SecuritySafeCritical]
//        [SecurityCritical]
//#endif
        protected virtual Type ViewGenericBaseType
        {
            get { return typeof(NHamlMvcView<>); }
        }

#if NET4
        [SecuritySafeCritical]
        [SecurityCritical]
#endif
        protected virtual Type GetViewBaseType(ControllerContext controllerContext)
        {
            var modelType = typeof(object);

            var viewData = controllerContext.Controller.ViewData;

            var viewContext = controllerContext as ViewContext;

            if ((viewContext != null) && (viewContext.ViewData != null))
            {
                viewData = viewContext.ViewData;
            }

            if ((viewData != null) && (viewData.Model != null))
            {
                modelType = viewData.Model.GetType();
            }

            return ViewGenericBaseType.MakeGenericType(modelType);
        }

#if NET4
        [SecuritySafeCritical]
        [SecurityCritical]
#endif
        protected virtual string VirtualPathToPhysicalPath(RequestContext context, string path)
        {
            return context.HttpContext.Request.MapPath(path);
        }
    }
}