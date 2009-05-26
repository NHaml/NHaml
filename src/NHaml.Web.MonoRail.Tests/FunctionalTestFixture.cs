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
        [Ignore]
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
        public virtual void RenderSectionComponent()
        {
            AssertRender("RenderSectionComponent");
        }

    }
}