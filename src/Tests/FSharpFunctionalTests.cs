using NHaml.Compilers.FSharp;
using NUnit.Framework;

namespace NHaml.Tests
{
#if (DEBUG)
    [TestFixture]
    public class FSharpFunctionalTests : FunctionalTestFixture
    {
        public override void SetUp()
        {
            base.SetUp();

            _templateEngine.Options.TemplateCompiler = new FSharpTemplateCompiler();

            _primaryTemplatesFolder = "FSharp";
        }
        public override void SwitchEval()
        {
            
        }

        [Ignore("Until i work out how to do multilevel lambdas")]
        [Test]
        public override void LambdaEval()
        {
            base.LambdaEval();
        }
    }
#endif
}