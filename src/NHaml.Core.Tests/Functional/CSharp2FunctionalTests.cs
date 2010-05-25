using NUnit.Framework;

namespace NHaml.Tests.Functional
{
    [TestFixture]
    public class CSharp2FunctionalTests : FunctionalTestFixture
    {
        public override void SetUp()
        {
            base.SetUp();

            _templateEngine.Options.TemplateContentProvider.AddPathSource( TemplatesFolder + @"CSharp2" );
        }

        [Test]
        public virtual void MetaNamespace()
        {
          AssertRender("MetaNamespace");
        }

        [Test]
        public virtual void MetaAssembly()
        {
          AssertRender("MetaAssembly");
        }
    }
}