using NHaml.Utils;
using NUnit.Framework;

namespace NHaml.Tests
{
  public class UtilsTests : TestFixtureBase
  {
    [Test]
    public void ClassNameProviderShouldReturnValidClassName()
    {
      var className = ClassNameProvider.MakeClassName( @" C:\abc ! XYZ\\#(){}[].hAml.." );

      Assert.AreEqual( className, "C__abc___XYZ__________hAml" );
    }

  }
}
