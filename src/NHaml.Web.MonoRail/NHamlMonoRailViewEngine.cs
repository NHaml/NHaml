using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Framework.Helpers;

namespace NHaml.Web.MonoRail
{
    public delegate void Action();
    public class NHamlMonoRailViewEngine: ViewEngineBase
    {
        public TemplateEngine TemplateEngine { get; private set; }

        public virtual string DefaultMaster { get; set; }

        public NHamlMonoRailViewEngine()
        {
            TemplateEngine = new TemplateEngine();
			TemplateEngine.AddExtension(new ComponentSectionExtension());
			TemplateEngine.AddExtension(new ComponentExtension());

            InitializeTemplateEngine();
        }
        public void SetViewSourceLoader(IViewSourceLoader loader)
        {
            ViewSourceLoader = loader;
        }

        private void InitializeTemplateEngine()
        {
            DefaultMaster = "application";

            TemplateEngine.AddUsing("System.Web");
            TemplateEngine.AddUsing("NHaml.Web.MonoRail");
            TemplateEngine.AddUsing("Castle.MonoRail.Framework.Helpers");
            

            TemplateEngine.AddReference(typeof(HtmlHelper).Assembly.Location);
            TemplateEngine.AddReference(typeof(NHamlMonoRailViewEngine).Assembly.Location);
            TemplateEngine.AddReference(typeof(DataContext).Assembly.Location);
            TemplateEngine.AddReference(typeof(Action).Assembly.Location);
            TemplateEngine.AddReference(typeof(Expression).Assembly.Location);
            
        }


        public override object CreateJSGenerator(JSCodeGeneratorInfo generatorInfo, IEngineContext context, IController controller, IControllerContext controllerContext)
        {
            return null;
        }

        public override void GenerateJS(string templateName, TextWriter output, JSCodeGeneratorInfo generatorInfo, IEngineContext context, IController controller, IControllerContext controllerContext)
        {
        }
        public string ViewRootDir
        {
            get { return ViewSourceLoader.ViewRootDir; }
        }
        public override void Process(string templateName, TextWriter output, IEngineContext context, IController controller, IControllerContext controllerContext)
        {
            var templateFileName = GetTemplateFileName(templateName);
            var layoutTemplatePath = GetLayoutFile(controllerContext);
            var compiledTemplate = TemplateEngine.Compile(templateFileName, layoutTemplatePath, typeof(NHamlMonoRailView));
            var template = (NHamlMonoRailView) compiledTemplate.CreateInstance();
            template.ViewEngine = this;
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
        private IList<string> GetLayoutFile(IControllerContext controllerContext)
        {
            var list = new List<string>();
            if (controllerContext.LayoutNames != null && controllerContext.LayoutNames.Length != 0)
            {
                for (var index = 0; index < controllerContext.LayoutNames.Length; index++)
                {
                    var layoutName = controllerContext.LayoutNames[index];
                    var layoutTemplate = layoutName;
                    if (!layoutTemplate.StartsWith("/"))
                    {
                        layoutTemplate = @"layouts\" + layoutTemplate;
                    }
                    layoutTemplate = layoutTemplate + ViewFileExtension;
                    list.Add(Path.Combine(ViewSourceLoader.ViewRootDir, layoutTemplate));
                }
            }
            return list;
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


        /// <summary>
        /// This takes a filename and return an instance of the view ready to be used.
        /// If the file does not exist, an exception is raised
        /// The cache is checked to see if the file has already been compiled, and it had been
        /// a check is made to see that the compiled instance is newer then the file's modification date.
        /// If the file has not been compiled, or the version on disk is newer than the one in memory, a new
        /// version is compiled.
        /// Finally, an instance is created and returned	
        /// </summary>
        public NHamlMonoRailView GetCompiledScriptInstance(string file)
        {
            // normalize filename - replace / or \ to the system path seperator
            var filename = file.Replace('/', Path.DirectorySeparatorChar)
                .Replace('\\', Path.DirectorySeparatorChar);

            filename = EnsurePathDoesNotStartWithDirectorySeparator(filename);
            Trace.WriteLine(string.Format("Getting compiled instnace of {0}", filename));

            return (NHamlMonoRailView) TemplateEngine.Compile(file).CreateInstance();
        }
    }
}
