using NUnit.Framework;

namespace NHaml.Tests
{
  [TestFixture]
  public class CS3FunctionalTests : FunctionalTests
  {
    public override void SetUp()
    {
      base.SetUp();

      _templateCompiler.CompilerVersion = "3.5";
      _templateCompiler.ViewBaseType = typeof(MockView);
    }

    [Test]
    public void LambdaEvalCS3()
    {
      AssertRender("LambdaEvalCS3", "LambdaEval", _templateCompiler);
    }
  }
}