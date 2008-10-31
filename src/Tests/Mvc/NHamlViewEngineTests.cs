using System.IO;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;

using Moq;

using NHaml.Web.Mvc;

using NUnit.Framework;

namespace NHaml.Tests.Mvc
{
  [TestFixture]
  public class NHamlViewEngineTests
  {
    private NHamlViewMvcEngine _engine;

    [SetUp]
    public void Setup()
    {
      _engine = new TestableNHamlViewMvcEngine();
    }

    [Test]
    public void CreateAndRenderPartialView()
    {
      using (var stringWriter = new StringWriter())
      {
        var controllerContext = CreateMockedControllerContext();

        var result = _engine.FindPartialView(controllerContext, "View");

        Assert.IsNotNull(result);

        var view = (CompiledTemplate)result.View;

        Assert.IsNotNull(view);

        view.Render(stringWriter);

        Assert.AreEqual("view\r\n", stringWriter.ToString());
      }
    }

    [Test]
    public void CreateAndRenderViewWithNullLayout()
    {
      CreateAndRenderView(null, "<p>\r\n    view\r\n</p>\r\n", "View");
    }

    [Test]
    public void CreateAndRenderViewWithEmptyLayout()
    {
      CreateAndRenderView(string.Empty, "<p>\r\n    view\r\n</p>\r\n", "View");
    }

    [Test]
    public void CreateAndRenderViewWithControllerLayout()
    {
      CreateAndRenderView("Controller", "<em>\r\n    view\r\n</em>\r\n", "View");
    }

    [Test]
    public void CreateAndRenderViewInSharedFolder()
    {
      CreateAndRenderView(null, "<p>\r\n    sharedview\r\n</p>\r\n", "SharedView");
    }

    [Test]
    public void CreateAndRenderViewUsingExplicitPath()
    {
      CreateAndRenderView(null, "<p>\r\n    specificpath\r\n</p>\r\n", "~/Views/specific.haml");
    }

    private void CreateAndRenderView(string withLayout, string expectedResult, string viewName)
    {
      using (var stringWriter = new StringWriter())
      {
        var controllerContext = CreateMockedControllerContext();

        var result = _engine.FindView(controllerContext, viewName, withLayout);

        Assert.IsNotNull(result);

        var view = (CompiledTemplate)result.View;

        Assert.IsNotNull(view);

        view.Render(stringWriter);

        Assert.AreEqual(expectedResult, stringWriter.ToString());
      }
    }

    private static ControllerContext CreateMockedControllerContext()
    {
      var contextMock = new Mock<HttpContextBase>();
      var controllerMock = new Mock<ControllerBase>(MockBehavior.Loose);
      var requestMock = new Mock<HttpRequestBase>();

      contextMock.Expect(x => x.Request).Returns(requestMock.Object);
      requestMock.Expect(x => x.MapPath(It.IsAny<string>())).Returns((string path) => TestableNHamlViewMvcEngine.FixPath(path));

      var routeData = new RouteData();
      routeData.Values.Add("controller", "Home");

      return new ControllerContext(contextMock.Object, routeData, controllerMock.Object);
    }

    private class TestableNHamlViewMvcEngine : NHamlViewMvcEngine
    {
      public TestableNHamlViewMvcEngine()
      {
        var vpp = new Mock<VirtualPathProvider>();
        vpp.Expect(x => x.FileExists(It.IsAny<string>())).Returns((string virtualPath) => File.Exists(FixPath(virtualPath)));
        VirtualPathProvider = vpp.Object;
      }

      public static string FixPath(string path)
      {
        return path.Replace("~/Views", "Templates/Mvc");
      }

      protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
      {
        partialPath = FixPath(partialPath);
        return base.CreatePartialView(controllerContext, partialPath);
      }

      protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
      {
        viewPath = FixPath(viewPath);
        masterPath = FixPath(masterPath);
        return base.CreateView(controllerContext, viewPath, masterPath);
      }
    }
  }
}