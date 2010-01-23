using NUnit.Framework;

namespace NHaml.Tests
{
    [TestFixture]
    public class MvcTests : TestFixtureMvcBase
    {
        [Test]
        public void RenderViewWithApplicationHamlWhenMasterIsNull()
        {
            AssertView( "Simple", null, "SimpleWithApplicationHamlMaster" );
        }

        [Test]
        public void RenderViewWhenMasterIsNullButApplicationHamlNotExists()
        {
            var save = _viewEngine.DefaultMaster;
            _viewEngine.DefaultMaster = "__DefaultMasterDoesNotExists";

            AssertView( "Simple", null, "RenderOnlyPartialView" );

            _viewEngine.DefaultMaster = save;
        }

        [Test]
        public void RenderViewWithApplicationHamlWhenMasterIsEmpty()
        {
            AssertView( "Simple", string.Empty, "SimpleWithApplicationHamlMaster" );
        }

        [Test]
        public void RenderViewWhenMasterIsEmptyButApplicationHamlNotExists()
        {
            var save = _viewEngine.DefaultMaster;
            _viewEngine.DefaultMaster = "__DefaultMasterDoesNotExists";

            AssertView( "Simple", string.Empty, "RenderOnlyPartialView" );

            _viewEngine.DefaultMaster = save;
        }

        [Test]
        public void RenderViewWithControllerNameAsMasterWhenMasterIsNull()
        {
            _controllerContext.RouteData.Values["controller"] = "Custom";
            AssertView( "Simple", null, "SimpleWithCustomMaster" );
        }

        [Test]
        public void RenderViewWithControllerNameAsMasterWhenMasterIsEmtpy()
        {
            _controllerContext.RouteData.Values["controller"] = "Custom";
            AssertView( "Simple", string.Empty, "SimpleWithCustomMaster" );
        }

        [Test]
        public void RenderSharedView()
        {
            AssertView( "SharedSimple", null, "SimpleWithApplicationHamlMaster" );
        }

        [Test]
        public void RenderViewWithCustomMaster()
        {
            AssertView( "Simple", "Custom", "SimpleWithCustomMaster" );
        }

        [Test]
        public void RenderViewWithCustomMasterAndVirtualPath()
        {
            AssertView( "~/Views/Mock/Simple.haml", "~/Views/Shared/Custom.haml", "SimpleWithCustomMaster" );
        }

        [Test]
        public void RenderOnlyPartialView()
        {
            AssertPartialView( "Simple", "RenderOnlyPartialView" );
        }

        [Test]
        public void HelperRendersPartial()
        {
            AssertPartialView( "HelperRendersPartial", "HelperRendersPartial" );
        }

        [Test]
        public void MasterDoesNotExists()
        {
            var view = _viewEngine.FindView( _controllerContext, "Simple", "__MasterDoesNotExists", false ).View;

            Assert.IsNull( view, "ViewEngine should not return a view when the master file does not exists" );
        }

        [Test]
        public void ViewDoesNotExists()
        {
            var view = _viewEngine.FindView( _controllerContext, "__ViewDoesNotExits", null, false ).View;

            Assert.IsNull( view, "ViewEngine should not return a view when the view file does not exists" );
        }
   
    }
}