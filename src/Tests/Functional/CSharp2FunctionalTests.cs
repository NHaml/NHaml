using NUnit.Framework;

namespace NHaml.Tests.Functional
{
    [TestFixture]
    public class CSharp2FunctionalTests : FunctionalTestFixture
    {
        public override void SetUp()
        {
            base.SetUp();

            _templateEngine.Options.TemplateContentProvider.AddPathSource( TemplatesFolder + @"CSharp2" );
        }
    }
}