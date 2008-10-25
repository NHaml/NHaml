using System;

using NHaml.BackEnds.Boo;

using NUnit.Framework;

namespace NHaml.Tests
{
  [TestFixture]
  public class BooFunctionalTests : FunctionalTests
  {
    public override void SetUp()
    {
      base.SetUp();

      _templateCompiler.CompilerBackEnd = new BooCompilerBackEnd();
      _templateCompiler.AddReference(typeof(Action).Assembly.Location);
      _templateCompiler.ViewBaseType = typeof(MockView);
    }

    [Test]
    public void LambdaEval()
    {
      AssertRender("LambdaEvalBoo", "LambdaEval", _templateCompiler);
    }

    [Test]
    public void AttributeEval()
    {
      AssertRender("AttributeEvalBoo", "AttributeEval", _templateCompiler);
    }

    [Test]
    public void SimpleEval()
    {
      AssertRender("SimpleEvalBoo", "SimpleEval", _templateCompiler);
    }

    [Test]
    public void SilentEval()
    {
      AssertRender("SilentEvalBoo", "SilentEval", _templateCompiler);
    }

    [Test]
    public void LoopEval()
    {
      AssertRender("LoopEvalBoo", "LoopEval", _templateCompiler);
    }

    [Test]
    public void Conditionals()
    {
      AssertRender("ConditionalsBoo", "Conditionals", _templateCompiler);
    }

    [Test]
    public void MultiLine()
    {
      AssertRender("MultiLineBoo", "MultiLine", _templateCompiler);
    }

    [Test]
    public void NullAttributes()
    {
      AssertRender("NullAttributesBoo", "NullAttributes", _templateCompiler);
    }

    [Test]
    public void Partials()
    {
      AssertRender("PartialsBoo", "Partials", _templateCompiler);
    }

    [Test]
    public void Preamble()
    {
      AssertRender("PreambleBoo", "Preamble", _templateCompiler);
    }

    [Test]
    public void Welcome()
    {
      AssertRender("WelcomeBoo", "Welcome", _templateCompiler);
    }

    [Test]
    public void Layout()
    {
      AssertRender("WelcomeBoo", "Application");
    }
  }
}