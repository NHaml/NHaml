using NHaml.Utils;
using NUnit.Framework;

namespace NHaml.Tests
{
    [TestFixture]
    public class UtilityTests
    {
        [Test]
        public void ClassNameProviderShouldReturnValidClassName()
        {
            var className = Utility.MakeClassName( @" C:\abc ! XYZ\\#(){}[].hAml.." );

            Assert.AreEqual( "C__abc___XYZ__________hAml", className );
        }

        [Test]
        public void ClassNameProviderShouldReturnValidClassNameWhenNoSlashesInPath()
        {
            var className = Utility.MakeClassName("abc");

            Assert.AreEqual( "abc", className );
        }
    }
}