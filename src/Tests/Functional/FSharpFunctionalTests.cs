using NHaml.Compilers.FSharp;
using NUnit.Framework;

namespace NHaml.Tests.Functional
{
#if (DEBUG)
    [TestFixture]
    public class FSharpFunctionalTests : FunctionalTestFixture
    {
        public override void SetUp()
        {
            base.SetUp();

            _templateEngine.Options.TemplateCompiler = new FSharpTemplateCompiler();
            _templateEngine.TemplateContentProvider.PathSources.Insert( 0, TemplatesFolder + @"FSharp" );
        }

        [Ignore( "SwitchEval is currently not supported" )]
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
#endif
}