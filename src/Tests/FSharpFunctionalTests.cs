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

            _templateEngine.TemplateCompiler = new FSharpTemplateCompiler();

            _primaryTemplatesFolder = "FSharp";
        }
        public override void SwitchEval()
        {
            
        }
        //TODO: not sure how to do this yet
        public override void LambdaEval()
        {
        }
    }
#endif
}