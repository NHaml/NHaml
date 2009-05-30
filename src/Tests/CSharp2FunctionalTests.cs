using NUnit.Framework;

namespace NHaml.Tests
{


    [TestFixture]
    public class CSharp2FunctionalTests : FunctionalTestFixture
    {
        public override void SetUp()
        {
            base.SetUp();

            _templateEngine.TemplateContentProvider.AddPathSource(@"Templates\CSharp2");
        }

    }
}