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

            _templateEngine.TemplateCompiler = new VisualBasicTemplateCompiler();

            _primaryTemplatesFolder = "VisualBasic";
        }

        public override void LambdaEval()
        {
        }
    }
}