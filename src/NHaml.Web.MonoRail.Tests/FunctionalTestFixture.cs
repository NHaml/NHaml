using NUnit.Framework;

namespace NHaml.Web.MonoRail.Tests
{
    [TestFixture]
    public abstract class FunctionalTestFixture : TestFixtureBase
    {
        [Test]
        public virtual void RenderBodyComponent()
        {
            AssertRender("RenderBodyComponent");
        }
        [Test]
        public virtual void RenderTextComponent()
        {
            AssertRender("RenderTextComponent");
        }
        [Test]
        public virtual void SimpleComponent()
        {
            AssertRender("SimpleComponent");
        }
        [Test]
        public virtual void RenderParametersComponent()
        {
            AssertRender("RenderParametersComponent");
        }
        [Test]
        public virtual void InnerComponent()
        {
            AssertRender("InnerComponent");
        }
        [Test]
        public virtual void RenderSectionComponent()
        {
            AssertRender("RenderSectionComponent");
        }
        [Test]
        public virtual void RenderPropertyBagComponent()
        {
            AssertRender("PropertyBagComponent");
        }

    }
}