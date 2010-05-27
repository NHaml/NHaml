using NUnit.Framework;
using NHaml.Core.Compilers;

namespace NHaml.Tests.Functional
{
    [TestFixture]
    public class CSharp3FunctionalTests : FunctionalTestFixture
    {
        public override void SetUp()
        {
            base.SetUp();

            _templateEngine.Options.TemplateCompilerType = typeof(CSharp3ClassBuilder);
            _templateEngine.Options.TemplateContentProvider.AddPathSource(TemplatesFolder + @"CSharp3");
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
