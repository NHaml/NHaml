using NUnit.Framework;

namespace NHaml.Tests.Issues.GC51
{
    [TestFixture]
    public class Fixture : TestFixtureMvcBase
    {
        public Fixture()
        {
            ExpectedFolder = TemplatesFolder = @"Issues\GC51\";
        }
       
        [Test]
        public void UnexpectedFileNotFoundWhenPartialUsedAfterMaster()
        {
            AssertView( "Content", "Application", "Output" );
        }
    }
}