using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.IO;
using System.Linq.Expressions;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Framework.Helpers;

namespace NHaml.Web.MonoRail
{
    public class NHamlMonoRailViewEngine: ViewEngineBase
    {

        private readonly TemplateEngine _templateEngine;

        public virtual string DefaultMaster { get; set; }

        public NHamlMonoRailViewEngine()
        {
            _templateEngine = new TemplateEngine();

            InitializeTemplateEngine();
        }
        private void InitializeTemplateEngine()
        {
            DefaultMaster = "application";

            _templateEngine.AddUsing("System.Web");
            _templateEngine.AddUsing("NHaml.Web.MonoRail");
            _templateEngine.AddUsing("Castle.MonoRail.Framework.Helpers");
            

            _templateEngine.AddReference(typeof(HtmlHelper).Assembly.Location);
            _templateEngine.AddReference(typeof(NHamlMonoRailViewEngine).Assembly.Location);
            _templateEngine.AddReference(typeof(DataContext).Assembly.Location);
            _templateEngine.AddReference(typeof(Action).Assembly.Location);
            _templateEngine.AddReference(typeof(Expression).Assembly.Location);
            
        }


        public override object CreateJSGenerator(JSCodeGeneratorInfo generatorInfo, IEngineContext context, IController controller, IControllerContext controllerContext)
        {
            return null;
        }

        public override void GenerateJS(string templateName, TextWriter output, JSCodeGeneratorInfo generatorInfo, IEngineContext context, IController controller, IControllerContext controllerContext)
        {
        }

        public override void Process(string templateName, TextWriter output, IEngineContext context, IController controller, IControllerContext controllerContext)
        {
            var templateFileName = GetTemplateFileName(templateName);
            var layoutTemplatePath = GetLayoutFile(controllerContext);
            var compiledTemplate = _templateEngine.Compile(templateFileName, layoutTemplatePath, typeof(NHamlMonoRailView));
            var template = (NHamlMonoRailView) compiledTemplate.CreateInstance();
            template.Render(context, output, controllerContext);
        }

        //Taken from boo view engine
        private string GetTemplateFileName(string templateName)
        {
            var file = templateName + ViewFileExtension;
            var filename = file.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
            filename = EnsurePathDoesNotStartWithDirectorySeparator(filename);
            return Path.Combine(ViewSourceLoader.ViewRootDir, filename);
        }

        //Taken from boo view engine
        private static string EnsurePathDoesNotStartWithDirectorySeparator(string path)
        {
            if ((path.Length > 0) && ((path[0] == Path.DirectorySeparatorChar) || (path[0] == Path.AltDirectorySeparatorChar)))
            {
                path = path.Substring(1);
            }
            return path;
        }

        //Taken from boo view engine
        private string GetLayoutFile(IControllerContext controllerContext)
        {
            if (controllerContext.LayoutNames != null && controllerContext.LayoutNames.Length != 0)
            {
                var layoutTemplate = controllerContext.LayoutNames[0];
                if (!layoutTemplate.StartsWith("/"))
                {
                    layoutTemplate = @"layouts\" + layoutTemplate;
                }
                layoutTemplate = layoutTemplate + ViewFileExtension;
                return Path.Combine(ViewSourceLoader.ViewRootDir, layoutTemplate) ;

            }
            return null;
        }
        protected virtual FileInfo CreateFileInfo(string viewRootDirectory, string viewPath)
        {
            if (Path.IsPathRooted(viewPath))
            {
                viewPath = viewPath.Substring(Path.GetPathRoot(viewPath).Length);
            }

            return new FileInfo(Path.Combine(viewRootDirectory, viewPath));
        }
        public override void Process(string templateName, string layoutName, TextWriter output, IDictionary<string, object> parameters)
        {
            var controllerContext = new ControllerContext();
            if (layoutName != null)
            {
                controllerContext.LayoutNames = new[] { layoutName };
            }
            foreach (var pair in parameters)
            {
                controllerContext.PropertyBag[pair.Key] = pair.Value;
            }
            Process(templateName, output, null, null, controllerContext);

        }

        public override void ProcessPartial(string partialName, TextWriter output, IEngineContext context, IController controller, IControllerContext controllerContext)
        {
            throw new NotImplementedException();
        }



        public override void RenderStaticWithinLayout(string contents, IEngineContext context, IController controller, IControllerContext controllerContext)
        {
            throw new NotImplementedException();
        }

        public override string ViewFileExtension
        {
            get { return ".haml"; }
        }

        public override bool SupportsJSGeneration
        {
            get { return false; }
        }

        public override string JSGeneratorFileExtension
        {
            get { return string.Empty; }
        }



    }
}
