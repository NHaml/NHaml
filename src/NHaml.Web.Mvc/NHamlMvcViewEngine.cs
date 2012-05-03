using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using System.Web.UI;
using System.IO;
using NHaml4;
using NHaml4.Walkers.CodeDom;
using NHaml4.Configuration;

namespace NHaml.Web.Mvc
{
    //[AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    //[AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class NHamlMvcViewEngine : VirtualPathProviderViewEngine
    {
        private readonly TemplateEngine _templateEngine;
        private readonly MapPathTemplateContentProvider _contentProvider;
        private readonly Type _baseType = typeof(NHamlMvcView<>);
        private bool _isMasterConfigured;
        public string DefaultMaster { get; set; }

        public NHamlMvcViewEngine()
        {
            InitializeBaseViewLocations();
            _contentProvider = new MapPathTemplateContentProvider();
            _templateEngine = XmlConfigurator.GetTemplateEngine(
                _contentProvider,
                GetDefaultUsings(),
                GetDefaultReferences());
            DefaultMaster = "Application";
        }

        private IList<string> GetDefaultUsings()
        {
            return new List<string> {
                "System.Web",
                "System.Web.Mvc",
                "System.Web.Mvc.Html",
                "System.Web.Routing",
                "NHaml.Web.Mvc" };
        }

        private IList<string> GetDefaultReferences()
        {
            var result = new List<string> {
                typeof(UserControl).Assembly.Location,
                typeof(RouteValueDictionary).Assembly.Location,
                typeof(DataContext).Assembly.Location,
                typeof(LinkExtensions).Assembly.Location,
                typeof(IView).Assembly.Location,
                typeof(NHamlMvcView).Assembly.Location
            };

            var referencedAssemblies = typeof(MvcHandler).Assembly.GetReferencedAssemblies();
            result.AddRange(referencedAssemblies.Select(x => Assembly.Load(x).Location));

            return result;
        }

        private void InitializeBaseViewLocations()
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
            if (!string.IsNullOrEmpty(masterName))
            {
                _isMasterConfigured = true;
                return base.FindView(controllerContext, viewName, masterName, useCache);
            }

            _isMasterConfigured = false;
            _contentProvider.SetRequestContext(controllerContext.RequestContext);
            var controllerName = controllerContext.RouteData.GetRequiredString("controller");
            var result = base.FindView(controllerContext, viewName, controllerName, useCache);

            if (result.View == null)
            {
                result = base.FindView(controllerContext, viewName, DefaultMaster, useCache);
            }

            return result.View == null ? base.FindView(controllerContext, viewName, null, useCache) : result;
        }

        protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
        {
            _contentProvider.SetRequestContext(controllerContext.RequestContext);
            string templatePath = VirtualPathToPhysicalPath(controllerContext.RequestContext, partialPath);
            var templateFactory = _templateEngine.GetCompiledTemplate(_contentProvider, templatePath, GetViewBaseType(controllerContext));
            return (IView)templateFactory.CreateTemplate();

            //return (IView)_templateEngine.Compile(
            //    VirtualPathToPhysicalPath(controllerContext.RequestContext,partialPath), // templatePath
            //    null, // master
            //    null, // defaultMaster
            //    GetViewBaseType(controllerContext) // BaseType
            //    ).CreateInstance();
        }

        protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
        {
            _contentProvider.SetRequestContext(controllerContext.RequestContext);
            viewPath = VirtualPathToPhysicalPath(controllerContext.RequestContext, viewPath);

            if (_isMasterConfigured)
            {
            //    masterPath = VirtualPathToPhysicalPath(controllerContext.RequestContext, masterPath);
            //    return (IView)_templateEngine.Compile(viewPath, masterPath, null, GetViewBaseType(controllerContext)).CreateInstance();
            }
            //else
            //{
            //    if (string.IsNullOrEmpty(masterPath))
            //    {
            return (IView)_templateEngine.GetCompiledTemplate(_contentProvider, viewPath, masterPath, GetViewBaseType(controllerContext)).
                    CreateTemplate();
            //    }
            //    else
            //    {
            //        masterPath = VirtualPathToPhysicalPath(controllerContext.RequestContext, masterPath);
            //        return (IView)_templateEngine.Compile(viewPath, null, masterPath, GetViewBaseType(controllerContext)).CreateInstance();
            //    }
            //}
        }

        protected virtual Type GetViewBaseType(ControllerContext controllerContext)
        {
            if (_baseType.IsGenericTypeDefinition)
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

                return _baseType.MakeGenericType(modelType);
            }
            else
            {
                return _baseType;
            }
        }

        protected virtual string VirtualPathToPhysicalPath(RequestContext context, string path)
        {
            return context.HttpContext.Request.MapPath(path);
        }
    }
}