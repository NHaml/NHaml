using System.IO;
using System.Web;
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
    [Test]
    public void CreateAndRenderPartialView()
    {
      using (var stringWriter = new StringWriter())
      {
        var controllerContext = CreateMockedControllerContext();
        var engine = new NHamlViewMvcEngine();

        var result = engine.FindPartialView(controllerContext, "View");

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
      CreateAndRenderView(null, "<p>\r\n    view\r\n</p>\r\n");
    }

    [Test]
    public void CreateAndRenderViewWithEmptyLayout()
    {
      CreateAndRenderView(string.Empty, "<p>\r\n    view\r\n</p>\r\n");
    }

    [Test]
    public void CreateAndRenderViewWithControllerLayout()
    {
      CreateAndRenderView("Controller", "<em>\r\n    view\r\n</em>\r\n");
    }

    private static void CreateAndRenderView(string withLayout, string expectedResult)
    {
      using (var stringWriter = new StringWriter())
      {
        var controllerContext = CreateMockedControllerContext();
        var engine = new NHamlViewMvcEngine();

        var result = engine.FindView(controllerContext, "View", withLayout);

        Assert.IsNotNull(result);

        var view = (CompiledTemplate)result.View;

        Assert.IsNotNull(view);

        view.Render(stringWriter);

        Assert.AreEqual(expectedResult, stringWriter.ToString());
      }
    }

    private static ControllerContext CreateMockedControllerContext()
    {
      var requestMock = new Mock<HttpRequestBase>();
      requestMock.Expect(x => x.MapPath(It.IsAny<string>()))
        .Returns((string path) => path.Replace("~/Views/Shared", "Templates/Mvc/Shared")
          .Replace("~/Views", "Templates/Mvc"));

      var contextMock = new Mock<HttpContextBase>();
      contextMock.Expect(x => x.Request).Returns(requestMock.Object);

      var controllerMock = new Mock<ControllerBase>(MockBehavior.Loose);

      var requestContext = new RequestContext(contextMock.Object, new RouteData());

      return new ControllerContext(requestContext, controllerMock.Object);
    }
  }
}