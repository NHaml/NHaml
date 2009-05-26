using NHaml.Compilers.CSharp3;

using NUnit.Framework;

namespace NHaml.Tests
{
    [TestFixture]
    public class CSharp3FunctionalTests : FunctionalTestFixture
    {
        public override void SetUp()
        {
            base.SetUp();

            _templateEngine.Options.TemplateCompiler = new CSharp3TemplateCompiler();

            _primaryTemplatesFolder = "CSharp3";
            _secondaryTemplatesFolder = "CSharp2";
        }
    }
}


