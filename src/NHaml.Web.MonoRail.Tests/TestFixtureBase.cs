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
        protected string Area;
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
            var services = new StubMonoRailServices
                               {
                                   UrlBuilder = new DefaultUrlBuilder(new StubServerUtility(), new StubRoutingEngine()),
                                   UrlTokenizer = new DefaultUrlTokenizer()
                               };
            var urlInfo = new UrlInfo(
                "example.org", "test", "/TestBrail", "http", 80,
                "http://test.example.org/test_area/test_controller/test_action.tdd",
                Area, ControllerName, Action, "tdd", "no.idea");
            StubEngineContext = new StubEngineContext(new StubRequest(), new StubResponse(), services, urlInfo);
            StubEngineContext.AddService<IUrlBuilder>(services.UrlBuilder);
            StubEngineContext.AddService<IUrlTokenizer>(services.UrlTokenizer);

            ViewComponentFactory = new DefaultViewComponentFactory();
            ViewComponentFactory.Service(StubEngineContext);
            ViewComponentFactory.Initialize();

            StubEngineContext.AddService<IViewComponentFactory>(ViewComponentFactory);
            ControllerContext = new ControllerContext
                                    {
                                        Helpers = Helpers, 
                                        PropertyBag = PropertyBag
                                    };
            StubEngineContext.CurrentControllerContext = ControllerContext;


            Helpers["urlhelper"] = Helpers["url"] = new UrlHelper(StubEngineContext);
            Helpers["htmlhelper"] = Helpers["html"] = new HtmlHelper(StubEngineContext);
            Helpers["dicthelper"] = Helpers["dict"] = new DictHelper(StubEngineContext);
            Helpers["DateFormatHelper"] = Helpers["DateFormat"] = new DateFormatHelper(StubEngineContext);



            var loader = new FileAssemblyViewSourceLoader("Views");
            _monoRailViewEngine = new NHamlMonoRailViewEngine();
            _monoRailViewEngine.TemplateEngine.Options.TemplateCompiler = new CSharp3TemplateCompiler();
            _monoRailViewEngine.SetViewSourceLoader(loader);
            _templateEngine = _monoRailViewEngine.TemplateEngine;
            _templateEngine.Options.TemplateBaseType = typeof( NHamlMonoRailView );
            


            ViewComponentFactory.Inspect(GetType().Assembly);

        }


        protected void AssertRender(string templateName)
        {
            using (var output = new StringWriter())
            {
                _monoRailViewEngine.Process(templateName, output, StubEngineContext, null, ControllerContext);

                AssertRender(output, templateName);
            }
        }


        protected static void AssertRender( StringWriter output, string expectedName )
        {
            Console.WriteLine( output );
            Assert.AreEqual( File.ReadAllText( string.Format("{0}{1}.xhtml", ExpectedFolder, expectedName) ), output.ToString() );
        }
    }
}


