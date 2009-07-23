using NUnit.Framework;

namespace NHaml.Tests.Issues.GC58
{
    [TestFixture]
    public class Fixture : TestFixtureMvcBase
    {
        public Fixture()
        {
            ExpectedFolder = TemplatesFolder = @"Issues\GC58\";
        }
       
        [Test]
        public void CodeNotEvaluatedAfterAttributes()
        {
            AssertView( "Content", "Application", "Output" );
        }
    }
}