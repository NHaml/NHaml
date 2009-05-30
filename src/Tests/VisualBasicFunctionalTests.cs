using NHaml.Compilers.VisualBasic;
using NUnit.Framework;

namespace NHaml.Tests
{
    [TestFixture]
    public class VisualBasicFunctionalTests : FunctionalTestFixture
    {
        public override void SetUp()
        {
            base.SetUp();

            _templateEngine.Options.TemplateCompiler = new VisualBasicTemplateCompiler();

            _templateEngine.TemplateContentProvider.AddPathSource(@"Templates\VisualBasic");
        }

        public override void LambdaEval()
        {
        }
    }
}