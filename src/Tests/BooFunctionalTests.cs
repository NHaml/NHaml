using NHaml.Compilers.Boo;

using NUnit.Framework;

namespace NHaml.Tests
{
    [TestFixture]
    public class BooFunctionalTests : FunctionalTestFixture
    {
        public override void SetUp()
        {
            base.SetUp();

            _templateEngine.TemplateCompiler = new BooTemplateCompiler();

            _primaryTemplatesFolder = "Boo";
        }

        public override void Empty()
        {
            // Boo doesn't handle this
        }

        public override void SwitchEval()
        {
            // Apparently "given" is not yet implemented
        }
    }
}