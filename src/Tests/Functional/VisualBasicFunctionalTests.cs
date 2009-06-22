using NHaml.Compilers.VisualBasic;
using NUnit.Framework;

namespace NHaml.Tests.Functional
{
    [TestFixture]
    public class VisualBasicFunctionalTests : FunctionalTestFixture
    {
        public override void SetUp()
        {
            base.SetUp();

            _templateEngine.Options.TemplateCompiler = new VisualBasicTemplateCompiler();

            _templateEngine.TemplateContentProvider.AddPathSource( TemplatesFolder + @"VisualBasic" );
        }

        [Ignore]
        public override void LambdaEval()
        {
        }
    }
}