using NHaml.Compilers.FSharp;
using NUnit.Framework;

namespace NHaml.Tests.Functional
{
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

        [Ignore("Until i work out how to do multilevel lambdas")]
        [Test]
        public override void LambdaEval()
        {
            base.LambdaEval();
        }
    }
}