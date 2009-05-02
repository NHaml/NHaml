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

    }
#endif
}