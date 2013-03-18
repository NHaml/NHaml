using System.IO;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using NHaml.Web.Mvc3;
using NUnit.Framework;

namespace System.Web.NHaml.Mvc3.Tests
{
    public class NHamlMvcViewEngine_Tests : NHamlMvcViewEngine
    {
        private Mock<HttpContextBase> _httpContext;
        private Mock<HttpRequestBase> _httpRequest;
        private RouteData _routeData;
        private FakeController _controller;

        public class FakeController : ControllerBase
        {
            protected override void ExecuteCore()
            { }
        }

        [SetUp]
        public void SetUp()
        {
            _httpContext = new Mock<HttpContextBase>();
            _httpRequest = new Mock<HttpRequestBase>();

            _httpContext.Setup(ctx => ctx.Request).Returns(_httpRequest.Object);
            _httpRequest.Setup(req => req.MapPath(It.IsAny<string>()))
             .Returns((String str) => str);

            _httpRequest.Setup(req => req.ApplicationPath)
                .Returns("/");

            _routeData = new RouteData();
            _controller = new FakeController();
        }

        [Test]
        public void CreateView_SimpleTags_GeneratesCorrectMarkup()
        {
            var controllerContext = new ControllerContext(_httpContext.Object, _routeData, _controller);
            const string viewPath = "SimpleView.haml";

            // Arrange
            var result = base.CreateView(controllerContext, viewPath, "");

            // Assert
            var writer = new StringWriter();
            var viewContext = new ViewContext
            {
                ViewData = new ViewDataDictionary()
            };
            result.Render(viewContext, writer);
            const string expected =
                @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">"
                + "\n\n<h1>Test</h1>";
            Assert.That(writer.ToString(), Is.EqualTo(expected));
        }

        [Test]
        public void CreateView_ViewWithMvcHelper_GeneratesCorrectMarkup()
        {
            var controllerContext = new ControllerContext(_httpContext.Object, _routeData, _controller);
            const string viewPath = "MvcHelperTestView.haml";

            // Arrange
            var result = base.CreateView(controllerContext, viewPath, "");

            // Assert
            var writer = new StringWriter();
            var viewContext = new ViewContext
            {
                ViewData = new ViewDataDictionary()
            };
            result.Render(viewContext, writer);
            const string expected =
                @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">"
                + "\n\n<a href=\"\">About Us</a>";
            Assert.That(writer.ToString(), Is.EqualTo(expected));
        }
    }
}
