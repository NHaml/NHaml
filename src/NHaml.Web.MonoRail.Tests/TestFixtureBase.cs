using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Framework.Helpers;
using Castle.MonoRail.Framework.Services;
using Castle.MonoRail.Framework.Test;
using NHaml.Compilers.CSharp3;
using NUnit.Framework;

namespace NHaml.Web.MonoRail.Tests
{
    public abstract class TestFixtureBase
    {
        public const string TemplatesFolder = @"Templates\";
        public const string ExpectedFolder = @"Expected\";

        protected TemplateEngine _templateEngine;

        protected HelperDictionary Helpers;
        protected Hashtable PropertyBag;
        protected DefaultViewComponentFactory ViewComponentFactory;
        protected string _primaryTemplatesFolder;
        protected string _secondaryTemplatesFolder;
        protected string Area = null;
        protected string ControllerName = "test_controller";
        protected StubEngineContext StubEngineContext;
        protected string Action = "test_action";
        private NHamlMonoRailViewEngine _monoRailViewEngine;
        protected ControllerContext ControllerContext;

        protected TestFixtureBase()
        {
            Trace.Listeners.Clear();
        }

        [SetUp]
        public virtual void SetUp()
        {



            PropertyBag = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
            Helpers = new HelperDictionary();
            var services = new StubMonoRailServices();
            services.UrlBuilder = new DefaultUrlBuilder(new StubServerUtility(), new StubRoutingEngine());
            services.UrlTokenizer = new DefaultUrlTokenizer();
            var urlInfo = new UrlInfo(
                "example.org", "test", "/TestBrail", "http", 80,
                "http://test.example.org/test_area/test_controller/test_action.tdd",
                Area, ControllerName, Action, "tdd", "no.idea");
            StubEngineContext = new StubEngineContext(new StubRequest(), new StubResponse(), services,
                                                      urlInfo);
            StubEngineContext.AddService<IUrlBuilder>(services.UrlBuilder);
            StubEngineContext.AddService<IUrlTokenizer>(services.UrlTokenizer);

            ViewComponentFactory = new DefaultViewComponentFactory();
            ViewComponentFactory.Service(StubEngineContext);
            ViewComponentFactory.Initialize();

            StubEngineContext.AddService<IViewComponentFactory>(ViewComponentFactory);
            ControllerContext = new ControllerContext();
            ControllerContext.Helpers = Helpers;
            ControllerContext.PropertyBag = PropertyBag;
            StubEngineContext.CurrentControllerContext = ControllerContext;


            Helpers["urlhelper"] = Helpers["url"] = new UrlHelper(StubEngineContext);
            Helpers["htmlhelper"] = Helpers["html"] = new HtmlHelper(StubEngineContext);
            Helpers["dicthelper"] = Helpers["dict"] = new DictHelper(StubEngineContext);
            Helpers["DateFormatHelper"] = Helpers["DateFormat"] = new DateFormatHelper(StubEngineContext);


            _monoRailViewEngine = new NHamlMonoRailViewEngine
            {
                TemplateEngine =
                {
                    TemplateCompiler = new CSharp3TemplateCompiler()
                }
            };
            _templateEngine = _monoRailViewEngine.TemplateEngine;
            _templateEngine.TemplateBaseType = typeof(NHamlMonoRailView);
            
            _primaryTemplatesFolder = "CSharp2";


            ViewComponentFactory.Inspect(GetType().Assembly);

        }


        protected void AssertRender( string templateName )
        {
            AssertRender( templateName, null );
        }

        protected void AssertRender( string templateName, string layoutName )
        {
            var expectedName = templateName;

            if( !string.IsNullOrEmpty( layoutName ) )
            {
                expectedName = layoutName;
            }

            AssertRender( templateName, layoutName, expectedName );
        }

        protected void AssertRender( string templateName, string layoutName, string expectedName )
        {
            var output = new StringWriter();
            var template = CreateTemplate( templateName, layoutName );

            template.Render(StubEngineContext, output, ControllerContext );

            AssertRender( output, expectedName );
        }

        protected NHamlMonoRailView CreateTemplate( string templateName, string layoutName )
        {
            var templatePath = string.Format("{0}{1}\\{2}.haml", TemplatesFolder, _primaryTemplatesFolder, templateName);

            if( !File.Exists( templatePath ) )
            {
                templatePath = string.Format("{0}{1}\\{2}.haml", TemplatesFolder, _secondaryTemplatesFolder, templateName);
            }

            if( !File.Exists( templatePath ) )
            {
                templatePath = string.Format("{0}{1}.haml", TemplatesFolder, templateName);
            }

            if( !string.IsNullOrEmpty( layoutName ) )
            {
                layoutName = string.Format("{0}{1}.haml", TemplatesFolder, layoutName);
            }

            var stopwatch = Stopwatch.StartNew();

            var compiledTemplate = _templateEngine.Compile(templatePath, layoutName);
            stopwatch.Stop();
            Debug.WriteLine(string.Format("Compile took {0} ms", stopwatch.ElapsedMilliseconds));
            return (NHamlMonoRailView) compiledTemplate.CreateInstance();
        }

        protected static void AssertRender( StringWriter output, string expectedName )
        {
            Console.WriteLine( output );
            Assert.AreEqual( File.ReadAllText( ExpectedFolder + expectedName + ".xhtml" ), output.ToString() );
        }
    }
}


