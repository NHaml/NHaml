using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Reflection;
using System.Security.Permissions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using System.Web.UI;
using NHaml.Core.Template;
using System.IO;
using NHaml.Core.Compilers;

namespace NHaml.Web.Mvc
{
    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class NHamlMvcViewEngine : VirtualPathProviderViewEngine
    {
        private readonly TemplateEngine _templateEngine = new TemplateEngine();
        private bool useDefault;
        public string DefaultMaster { get; set; }

        private MapPathTemplateContentProvider _contentProvider;

        public NHamlMvcViewEngine()
        {
            InitializeTemplateEngine();
            InitializeViewLocations();
        }

        private void InitializeTemplateEngine()
        {
            DefaultMaster = "Application";

            _templateEngine.Options.AddUsing( "System.Web" );
            _templateEngine.Options.AddUsing( "System.Web.Mvc" );
            _templateEngine.Options.AddUsing( "System.Web.Mvc.Html" );
            _templateEngine.Options.AddUsing( "System.Web.Routing" );
            _templateEngine.Options.AddUsing( "NHaml.Web.Mvc" );

            foreach (var referencedAssembly in typeof(MvcHandler).Assembly.GetReferencedAssemblies())
            {
                _templateEngine.Options.AddReference(Assembly.Load(referencedAssembly).Location);
            } 
            _templateEngine.Options.AddReference( typeof( UserControl ).Assembly.Location );
            _templateEngine.Options.AddReference( typeof( RouteValueDictionary ).Assembly.Location );
            _templateEngine.Options.AddReference( typeof( DataContext ).Assembly.Location );
            _templateEngine.Options.AddReference( typeof( LinkExtensions ).Assembly.Location );
            _templateEngine.Options.AddReference( typeof( IView ).Assembly.Location );

            if (_templateEngine.Options.TemplateBaseType == typeof(Template))
            {
                _templateEngine.Options.TemplateBaseType = typeof(NHamlMvcView<>);
            }

            _contentProvider = new MapPathTemplateContentProvider();
            _templateEngine.Options.TemplateContentProvider = _contentProvider;
        }

        public TemplateEngine TemplateEngine
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

            AreaViewLocationFormats = new[]
            {
                "~/Areas/{2}/Views/{1}/{0}.haml",
                "~/Areas/{2}/Views/Shared/{0}.haml"
            };

            AreaMasterLocationFormats = new [] 
 	        { 
 	            "~/Areas/{2}/Views/Shared/{0}.haml" 
 	        };

            PartialViewLocationFormats = new[]
            {
                "~/Views/{1}/_{0}.haml",
                "~/Views/Shared/_{0}.haml",
                "~/Views/{1}/{0}.haml",
                "~/Views/Shared/{0}.haml"
            };

            AreaPartialViewLocationFormats = new[]
            {
                "~/Areas/{2}/Views/{1}/_{0}.haml",
                "~/Areas/{2}/Views/Shared/_{0}.haml",
                "~/Areas/{2}/Views/{1}/{0}.haml",
                "~/Areas/{2}/Views/Shared/{0}.haml"
            };
        }

        public override ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            _contentProvider.SetRequestContext(controllerContext.RequestContext);
            useDefault = true;
            if (string.IsNullOrEmpty(masterName))
            {
                useDefault = false;
                var controllerName = controllerContext.RouteData.GetRequiredString("controller");
                var result = base.FindView(controllerContext, viewName, controllerName, useCache);

                if (result.View == null)
                {
                    result = base.FindView(controllerContext, viewName, DefaultMaster, useCache);
                }

                return result.View == null ? base.FindView(controllerContext, viewName, null, useCache) : result;
            }

            return base.FindView(controllerContext, viewName, masterName, useCache);
        }

        protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
        {
            _contentProvider.SetRequestContext(controllerContext.RequestContext);
            return (IView)_templateEngine.Compile(VirtualPathToPhysicalPath(controllerContext.RequestContext,partialPath),null,null,GetViewBaseType(controllerContext)).CreateInstance();
        }

        protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
        {
            _contentProvider.SetRequestContext(controllerContext.RequestContext);
            viewPath = VirtualPathToPhysicalPath(controllerContext.RequestContext, viewPath);
            if (useDefault)
            {
                masterPath = VirtualPathToPhysicalPath(controllerContext.RequestContext, masterPath);
                return (IView)_templateEngine.Compile(viewPath, masterPath, null, GetViewBaseType(controllerContext)).CreateInstance();
            }
            else
            {
                if (string.IsNullOrEmpty(masterPath))
                {
                    return (IView)_templateEngine.Compile(viewPath, null, null, GetViewBaseType(controllerContext)).CreateInstance();
                }
                else
                {
                    masterPath = VirtualPathToPhysicalPath(controllerContext.RequestContext, masterPath);
                    return (IView)_templateEngine.Compile(viewPath, null, masterPath, GetViewBaseType(controllerContext)).CreateInstance();
                }
            }
        }

        protected virtual Type GetViewBaseType(ControllerContext controllerContext)
        {
            if (_templateEngine.Options.TemplateBaseType.IsGenericTypeDefinition)
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

                return _templateEngine.Options.TemplateBaseType.MakeGenericType(modelType);
            }
            else
            {
                return _templateEngine.Options.TemplateBaseType;
            }
        }

        protected virtual string VirtualPathToPhysicalPath(RequestContext context, string path)
        {
            return context.HttpContext.Request.MapPath(path);
        }
    }
}