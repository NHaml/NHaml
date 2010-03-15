using NUnit.Framework;

namespace NHaml.Tests.Issues.GC62
{
    [TestFixture]
    public class Fixture : TestFixtureMvcBase
    {
        public Fixture()
        {
            ExpectedFolder = TemplatesFolder = @"Issues\GC62\";
        }
       
        [Test]
        public void MissingIndent()
        {
            AssertView( "Content", "Application", "Output" );
        }
    }
}