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
    private StubViewEngine _viewEngine;
    private ControllerContext _controllerContext;
    private StringWriter _output;

    public override void SetUp()
    {
      ViewEngines.Engines.Clear();

      _viewEngine = new StubViewEngine();

      var mockContext = new Mock<HttpContextBase>();
      var mockHttpRequest = new Mock<HttpRequestBase>();
      var mockHttpResponse = new Mock<HttpResponseBase>();

      _output = new StringWriter();

      ViewEngines.Engines.Add(_viewEngine);

      mockHttpRequest.Expect(req => req.MapPath(It.IsAny<string>()))
        .Returns<string>(FakeServerMapPath);
      mockHttpResponse.Expect(rsp => rsp.Output).Returns(_output);
      mockContext.Expect(ctx => ctx.Response).Returns(mockHttpResponse.Object);
      mockContext.Expect(ctx => ctx.Request).Returns(mockHttpRequest.Object);

      var routeData = new RouteData();
      routeData.Values.Add("controller", "Mock");

      _controllerContext = new ControllerContext(mockContext.Object, routeData, new Mock<ControllerBase>().Object);
    }

    [Test]
    public void RenderViewWithApplicationHamlWhenMasterIsNull()
    {
      AssertView("Simple", null, "SimpleWithApplicationHamlMaster");
    }

    [Test]
    public void RenderViewWhenMasterIsNullButApplicationHamlNotExists()
    {
      var save = _viewEngine.DefaultMaster;
      _viewEngine.DefaultMaster = "__DefaultMasterDoseNotExists";

      AssertView("Simple", null, "RenderOnlyPartialView");

      _viewEngine.DefaultMaster = save;
    }

    [Test]
    public void RenderViewWithApplicationHamlWhenMasterIsEmpty()
    {
      AssertView("Simple", string.Empty, "SimpleWithApplicationHamlMaster");
    }

    [Test]
    public void RenderViewWhenMasterIsEmptyButApplicationHamlNotExists()
    {
      var save = _viewEngine.DefaultMaster;
      _viewEngine.DefaultMaster = "__DefaultMasterDoseNotExists";

      AssertView("Simple", string.Empty, "RenderOnlyPartialView");

      _viewEngine.DefaultMaster = save;
    }

    [Test]
    public void RenderViewWithControllerNameAsMasterWhenMasterIsNull()
    {
      _controllerContext.RouteData.Values["controller"] = "Custom";
      AssertView("Simple", null, "SimpleWithCustomMaster");
    }

    [Test]
    public void RenderViewWithControllerNameAsMasterWhenMasterIsEmtpy()
    {
      _controllerContext.RouteData.Values["controller"] = "Custom";
      AssertView("Simple", string.Empty, "SimpleWithCustomMaster");
    }

    [Test]
    public void RenderSharedView()
    {
      AssertView("SharedSimple", null, "SimpleWithApplicationHamlMaster");
    }

    [Test]
    public void RenderViewWithCustomMaster()
    {
      AssertView("Simple", "Custom", "SimpleWithCustomMaster");
    }

    [Test]
    public void RenderViewWithCustomMasterAndVirtualPath()
    {
      AssertView("~/Views/Mock/Simple.haml", "~/Views/Shared/Custom.haml", "SimpleWithCustomMaster");
    }

    [Test]
    public void RenderOnlyPartialView()
    {
      AssertPartialView("Simple", "RenderOnlyPartialView");
    }

    [Test]
    public void HelperRendersPartial()
    {
      AssertPartialView("HelperRendersPartial", "HelperRendersPartial");
    }

    [Test]
    public void MasterDoseNotExists()
    {
      var view = _viewEngine.FindView(_controllerContext, "Simple", "__MasterDoseNotExists", false).View;

      Assert.IsNull(view, "ViewEngine should not return a view when the master file dose not exists");
    }

    [Test]
    public void ViewDoseNotExists()
    {
      var view = _viewEngine.FindView(_controllerContext, "__ViewDoseNotExits", null, false).View;

      Assert.IsNull(view, "ViewEngine should not return a view when the view file dose not exists");
    }

    private void AssertView(string viewName, string masterName, string expectedName)
    {
      var view = _viewEngine.FindView(_controllerContext, viewName, masterName, false).View;

      AssertView(view, expectedName);
    }

    private void AssertPartialView(string viewName, string expectedName)
    {
      var view = _viewEngine.FindPartialView(_controllerContext, viewName, false).View;

      AssertView(view, expectedName);
    }

    private void AssertView(IView view, string expectedName)
    {
      Assert.IsNotNull(view, "ViewEngine dose not returned a view");
      //ControllerContext , IView , ViewDataDictionary , TempDataDictionary 
      var mockViewContext = new Mock<ViewContext>();
        mockViewContext.ExpectGet(x => x.View).Returns(view);
        mockViewContext.ExpectGet(x => x.ViewData).Returns(new ViewDataDictionary());
        mockViewContext.ExpectGet(x => x.TempData).Returns(new TempDataDictionary());
      view.Render(mockViewContext.Object, _output);

      AssertRender(_output, "Mvc\\" + expectedName);
    }

    private static string FakeServerMapPath(string path)
    {
      var templateFolder = TemplatesFolder + "Mvc\\";

      if (string.IsNullOrEmpty(path))
      {
        return templateFolder;
      }

      return templateFolder + path.Replace("~/Views/", "").Replace("/", "\\");
    }

    private class StubViewEngine : NHamlMvcViewEngine
    {
      public StubViewEngine()
      {
        VirtualPathProvider = new StubVirtualPathProvider();
      }

      private class StubVirtualPathProvider : VirtualPathProvider
      {
        public override bool FileExists(string virtualPath)
        {
          return File.Exists(FakeServerMapPath(virtualPath));
        }
      }
    }
  }
}