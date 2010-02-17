using NUnit.Framework;

namespace NHaml.Tests.Issues.GC50
{
    [TestFixture]
    public class Fixture : TestFixtureMvcBase
    {
        public Fixture()
        {
            ExpectedFolder = TemplatesFolder = @"Issues\GC50\";
        }
       
        [Test]
        public void UnexpectedFileNotFoundWhenPartialUsedAfterMaster()
        {
            AssertView( "Content", "Application", "Output" );
        }
    }
}