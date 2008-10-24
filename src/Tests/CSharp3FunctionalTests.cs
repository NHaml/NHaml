using NHaml.Backends.CSharp3;

using NUnit.Framework;

using System;

namespace NHaml.Tests
{
  [TestFixture]
  public class CSharp3FunctionalTests : CSharp2FunctionalTests
  {
    public override void SetUp()
    {
      base.SetUp();

      _templateCompiler.CompilerBackend = new CSharp3CompilerBackend();
      _templateCompiler.AddReference(typeof(Action).Assembly.Location);
      _templateCompiler.ViewBaseType = typeof(MockView);
    }

    [Test]
    public void LambdaEvalCS3()
    {
      AssertRender("LambdaEvalCS3", "LambdaEval", _templateCompiler);
    }
  }
}