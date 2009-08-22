using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data.Linq;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using Castle.Core;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Framework.Helpers;
using NHaml.Compilers;

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

            InitializeTemplateEngine();
        }
        public void SetViewSourceLoader(IViewSourceLoader loader)
        {
            ViewSourceLoader = loader;
        }

        private void InitializeTemplateEngine()
        {
            
            DefaultMaster = "Application";


            var options = TemplateEngine.Options;
            options.BeforeCompile = BeforeCompile;
            options.AddRule(new ComponentSectionMarkupRule());
            options.AddRule(new ComponentMarkupRule());

            options.AddUsing( "System.Web" );
            options.AddUsing( "NHaml.Web.MonoRail" );
            options.AddUsing( "Castle.Core" );
            options.AddUsing( "Castle.MonoRail.Framework.Helpers" );


            options.AddReference(typeof(IServiceEnabledComponent).Assembly.Location);
            options.AddReference( typeof( HtmlHelper ).Assembly.Location );
            options.AddReference( typeof( NHamlMonoRailViewEngine ).Assembly.Location );
            options.AddReference( typeof( DataContext ).Assembly.Location );
            options.AddReference( typeof( Action ).Assembly.Location );
            options.AddReference( typeof( Expression ).Assembly.Location );

            if (options.TemplateBaseType == null)
            {
                options.TemplateBaseType = typeof (NHamlMonoRailView);
            }
            //    TemplateEngine.TemplateContentProvider.AddPathSource(@"Views/Components");
        }

        private static void BeforeCompile(TemplateClassBuilder templateClassBuilder, object context)
        {
            var templateBuilderContext = (TemplateBuilderContext) context;
            var codeDomClassBuilder = (CodeDomClassBuilder)templateClassBuilder;

            foreach (var pair in templateBuilderContext.Helpers)
            {
                var property = new CodeMemberField
                               {
                                   Name = pair.Key,
                                   Type = new CodeTypeReference(pair.Value),
                                   Attributes = MemberAttributes.Public,
                               };
                codeDomClassBuilder.Members.Add(property);
            }
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
            var sources = GetLayoutFile(controllerContext);
            var templateFileName = GetTemplateFileName(templateName);
            sources.Add(templateFileName);

            var template = GetTemplate(controllerContext, sources);
            template.Render(context, output, controllerContext);
        }

        private NHamlMonoRailView GetTemplate(IControllerContext controllerContext, IList<string> sources)
        {
            foreach (AbstractHelper abstractHelper in controllerContext.Helpers.Values)
            {
                TemplateEngine.Options.AddReferences(abstractHelper.GetType());
            }
            var templateBuilderContext = new TemplateBuilderContext();
            foreach (var key in controllerContext.Helpers.Keys)
            {
                templateBuilderContext.Helpers.Add((string) key, controllerContext.Helpers[key].GetType());
	}
            var compiledTemplate = TemplateEngine.Compile(TemplateEngine.Options.TemplateBaseType, TemplateEngine.ConvertPathsToViewSources(sources), templateBuilderContext);
            var template = (NHamlMonoRailView) compiledTemplate.CreateInstance();
            template.ViewEngine = this;
            var tempalteType = template.GetType();
            foreach (var key in controllerContext.Helpers.Keys)
            {
                var property = tempalteType.GetField((string) key);
                property.SetValue(template, controllerContext.Helpers[key]);
            }
            return template;
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
        public NHamlMonoRailView GetCompiledScriptInstance(string file, IControllerContext controllerContext)
        {
            // normalize filename - replace / or \ to the system path seperator
            var filename = file.Replace('/', Path.DirectorySeparatorChar)
                .Replace('\\', Path.DirectorySeparatorChar);

            filename = EnsurePathDoesNotStartWithDirectorySeparator(filename);
            Trace.WriteLine(string.Format("Getting compiled instance of {0}", filename));

            GetTemplate(controllerContext, new []{filename});
            var template = TemplateEngine.Compile(file).CreateInstance();
            return (NHamlMonoRailView)template;
        }
        public override void Process(string templateName, string layoutName, TextWriter output, IDictionary<string, object> parameters)
        {
            var controllerContext = GetControllerContext(layoutName, parameters);
            Process(templateName, output, null, null, controllerContext);
        }

        public static ControllerContext GetControllerContext(string layoutName, IDictionary<string, object> parameters)
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
            return controllerContext;
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

    internal class TemplateBuilderContext
    {
        public TemplateBuilderContext()
        {
            Helpers = new Dictionary<string, Type>();
        }
        public Dictionary<string, Type> Helpers { get; set; }
    }
}
