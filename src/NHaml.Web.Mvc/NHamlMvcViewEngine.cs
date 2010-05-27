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
                _templateEngine.Options.TemplateBaseType = typeof(NHamlMvcView);
            }

            _templateEngine.Options.TemplateCompilerType = typeof(CSharp3ClassBuilder);
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
            return (IView)_templateEngine.Compile(VirtualPathToPhysicalPath(controllerContext.RequestContext,partialPath)).CreateInstance();
        }

        protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
        {
            viewPath = VirtualPathToPhysicalPath(controllerContext.RequestContext, viewPath);
            if (useDefault)
            {
                masterPath = VirtualPathToPhysicalPath(controllerContext.RequestContext, masterPath);
                return (IView)_templateEngine.Compile(viewPath, masterPath, null).CreateInstance();
            }
            else
            {
                if (string.IsNullOrEmpty(masterPath))
                {
                    return (IView)_templateEngine.Compile(viewPath, null, null).CreateInstance();
                }
                else
                {
                    masterPath = VirtualPathToPhysicalPath(controllerContext.RequestContext, masterPath);
                    return (IView)_templateEngine.Compile(viewPath, null, masterPath).CreateInstance();
                }
            }
        }

        protected virtual string VirtualPathToPhysicalPath(RequestContext context, string path)
        {
            return context.HttpContext.Request.MapPath(path);
        }
    }
}