using NUnit.Framework;

namespace NHaml.Tests.Issues.GC57
{
    [TestFixture]
    public class Fixture : TestFixtureMvcBase
    {
        public Fixture()
        {
            ExpectedFolder = TemplatesFolder = @"Issues\GC57\";
        }
       
        [Test]
        public void MissingNewLine()
        {
            AssertView( "Content", "Application", "Output" );
        }
    }
}