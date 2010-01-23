using System.IO;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;

using NHaml.Web.Mvc;

using NUnit.Framework;

namespace NHaml.Tests
{
    public abstract class TestFixtureMvcBase : TestFixtureBase
    {
        protected StubViewEngine _viewEngine;
        protected ControllerContext _controllerContext;

        protected TestFixtureMvcBase()
        {
            TemplatesFolder = TemplatesFolder + @"Mvc\";
            ExpectedFolder = ExpectedFolder + @"Mvc\";
        }

        public StringWriter Output { get; set; }

        public override void SetUp()
        {
            ViewEngines.Engines.Clear();

            _viewEngine = new StubViewEngine(this);

            var mockContext = new Mock<HttpContextBase>();
            var mockHttpRequest = new Mock<HttpRequestBase>();
            var mockHttpResponse = new Mock<HttpResponseBase>();

            Output = new StringWriter();

            ViewEngines.Engines.Add(_viewEngine);

            mockHttpRequest.Expect(req => req.MapPath(It.IsAny<string>()))
                .Returns<string>(FakeServerMapPath);
            mockHttpResponse.Expect(rsp => rsp.Output).Returns(Output);
            mockContext.Expect(ctx => ctx.Response).Returns(mockHttpResponse.Object);
            mockContext.Expect(ctx => ctx.Request).Returns(mockHttpRequest.Object);

            var routeData = new RouteData();
            routeData.Values.Add("controller", "Mock");

            _controllerContext = new ControllerContext(mockContext.Object, routeData, new Mock<ControllerBase>().Object);
        }

        protected void AssertView(string viewName, string masterName, string expectedName)
        {
            var view = _viewEngine.FindView(_controllerContext, viewName, masterName, false).View;

            AssertView(view, expectedName);
        }

        protected void AssertPartialView( string viewName, string expectedName )
        {
            var view = _viewEngine.FindPartialView(_controllerContext, viewName, false).View;

            AssertView(view, expectedName);
        }

        protected void AssertView( IView view, string expectedName )
        {
            Assert.IsNotNull(view, "ViewEngine does not returned a view");
            var mockViewContext = new Mock<ViewContext>();
            mockViewContext.ExpectGet( x => x.View ).Returns( view );
            mockViewContext.ExpectGet( x => x.Controller ).Returns( _controllerContext.Controller );
            mockViewContext.ExpectGet(x => x.HttpContext).Returns(_controllerContext.HttpContext);
            mockViewContext.ExpectGet(x => x.ViewData).Returns(new ViewDataDictionary());
            mockViewContext.ExpectGet(x => x.TempData).Returns(new TempDataDictionary());
            mockViewContext.ExpectGet(x => x.RouteData).Returns(_controllerContext.RouteData);
            mockViewContext.ExpectGet(x => x.Writer).Returns(Output);
            view.Render(mockViewContext.Object, Output);

            AssertRender(Output, expectedName);
        }

        private string FakeServerMapPath(string path)
        {
            var templateFolder = TemplatesFolder;

            if (string.IsNullOrEmpty(path))
            {
                return templateFolder;
            }

            return templateFolder + path.Replace("~/Views/", "").Replace("/", "\\");
        }

        protected class StubViewEngine : NHamlMvcViewEngine
        {
            public StubViewEngine(TestFixtureMvcBase mvcBase)
            {
                VirtualPathProvider = new StubVirtualPathProvider(mvcBase);
            }

            private class StubVirtualPathProvider : VirtualPathProvider
            {
                private readonly TestFixtureMvcBase _mvcBase;

                public StubVirtualPathProvider(TestFixtureMvcBase mvcBase)
                {
                    _mvcBase = mvcBase;
                }

                public override bool FileExists(string virtualPath)
                {
                    return File.Exists(_mvcBase.FakeServerMapPath(virtualPath));
                }
            }
        }
    }
}