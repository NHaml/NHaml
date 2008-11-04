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
      var className = Utility.MakeClassName(@" C:\abc ! XYZ\\#(){}[].hAml..");

      Assert.AreEqual(className, "C__abc___XYZ__________hAml");
    }
  }
}