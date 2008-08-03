using NUnit.Framework;

namespace NHaml.Tests
{
  [TestFixture]
  public class CS2FunctionalTests : FunctionalTests
  {
    public override void SetUp()
    {
      base.SetUp();

      _templateCompiler.CompilerVersion = "2.0";
    }
  }
}