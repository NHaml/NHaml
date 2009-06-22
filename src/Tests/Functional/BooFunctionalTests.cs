using NHaml.Compilers.Boo;
using NUnit.Framework;

namespace NHaml.Tests.Functional
{
    [TestFixture]
    public class BooFunctionalTests : FunctionalTestFixture
    {
        public override void SetUp()
        {
            base.SetUp();

            _templateEngine.Options.TemplateCompiler = new BooTemplateCompiler();
            _templateEngine.TemplateContentProvider.AddPathSource(TemplatesFolder+@"Boo");
        }

        [Ignore( "Boo doesn't handle this" )]
        public override void Empty()
        {
        }

        [Ignore("Apparently 'given' is not yet implemented")]
        public override void SwitchEval()
        {
        }
    }
}