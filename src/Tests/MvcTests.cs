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
  [TestFixture]
  public sealed class MvcTests : TestFixtureBase
  {
    private MockViewEngine _viewEngine;
    private ControllerContext _controllerContext;
    private StringWriter _output;

    public override void SetUp()
    {
      ViewEngines.Engines.Clear();

      _viewEngine = new MockViewEngine();

      ViewEngines.Engines.Add(_viewEngine);

      var mockContext = new Mock<HttpContextBase>();
      var mockHttpResponse = new Mock<HttpResponseBase>();

      _output = new StringWriter();

      mockHttpResponse.Expect(rsp => rsp.Output).Returns(_output);
      mockContext.Expect(ctx => ctx.Response).Returns(mockHttpResponse.Object);
      mockContext.Expect(ctx => ctx.Request).Returns(new Mock<HttpRequestBase>().Object);

      var routeData = new RouteData();
      routeData.Values.Add("controller", "Mock");

      _controllerContext = new ControllerContext(mockContext.Object, routeData, new Mock<ControllerBase>().Object);
    }

    [Test]
    public void MvcPartialRendering()
    {
      AssertView("MvcPartial");
    }

    private void AssertView(string viewName)
    {
      var view = _viewEngine.FindView(_controllerContext, viewName, null).View;

      var mockViewContext = new Mock<ViewContext>(_controllerContext, view,
        new ViewDataDictionary(), new TempDataDictionary());

      view.Render(mockViewContext.Object, _output);

      AssertRender(_output, viewName);
    }

    private class MockViewEngine : NHamlMvcViewEngine
    {
      public MockViewEngine()
      {
        VirtualPathProvider = new MockVirtualPathProvider();
      }

      protected override string VirtualPathToPhysicalPath(RequestContext context, string path)
      {
        if (path.Contains(FakeMasterName))
        {
          return null;
        }

        return path.Replace("~/Views/Mock", TemplatesFolder);
      }
    }

    public class MockVirtualPathProvider : VirtualPathProvider
    {
      public override bool FileExists(string virtualPath)
      {
        return true;
      }
    }
  }
}