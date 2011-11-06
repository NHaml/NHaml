#if (DEBUG)
using NHaml.Compilers.FSharp;
using NUnit.Framework;

namespace NHaml.Tests.Functional
{
    [TestFixture]
    [Ignore("FSharp support is really buggy ATM")]
    public class FSharpFunctionalTests : FunctionalTestFixture
    {
        public override void SetUp()
        {
            base.SetUp();

            _templateEngine.Options.TemplateCompilerType = typeof(FSharpClassBuilder);
            _templateEngine.Options.TemplateContentProvider.AddPathSource(TemplatesFolder + @"FSharp");
        }

        [Ignore("SwitchEval is currently not supported")]
        public override void SwitchEval()
        {

        }

        [Ignore("Until debugging with fsharp works")]
        public override void WithRunTimeException()
        {
            base.WithRunTimeException();
        }

        [Ignore("Until i work out how to do multilevel lambdas")]
        [Test]
        public override void LambdaEval()
        {
            base.LambdaEval();
        }
    }
}
#endif
