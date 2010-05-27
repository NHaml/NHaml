using NUnit.Framework;
using NHaml.Compilers.VisualBasic;

namespace NHaml.Tests.Functional
{
    [TestFixture]
    public class VisualBasicFunctionalTests : FunctionalTestFixture
    {
        public override void SetUp()
        {
            base.SetUp();

            _templateEngine.Options.TemplateCompilerType = typeof(VisualBasicClassBuilder);
            _templateEngine.Options.TemplateContentProvider.AddPathSource(TemplatesFolder + @"VisualBasic");
        }

        [Ignore]
        public override void LambdaEval()
        {
        }
 
    }
}