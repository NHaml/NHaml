using NHaml.Backends.CSharp2;

using NUnit.Framework;

namespace NHaml.Tests
{
  [TestFixture]
  public class CSharp2FunctionalTests : FunctionalTests
  {
    public override void SetUp()
    {
      base.SetUp();

      _templateCompiler.CompilerBackend = new CSharp2CompilerBackend();
    }

    [Test]
    public void LambdaEval()
    {
      AssertRender("LambdaEvalCS2", "LambdaEval", _templateCompiler);
    }

    [Test]
    public void AttributeEval()
    {
      AssertRender("AttributeEvalCS2", "AttributeEval", _templateCompiler);
    }

    [Test]
    public void SimpleEval()
    {
      AssertRender("SimpleEvalCS2", "SimpleEval", _templateCompiler);
    }

    [Test]
    public void SilentEval()
    {
      AssertRender("SilentEvalCS2", "SilentEval", _templateCompiler);
    }

    [Test]
    public void LoopEval()
    {
      AssertRender("LoopEvalCS2", "LoopEval", _templateCompiler);
    }

    [Test]
    public void SwitchEval()
    {
      AssertRender("SwitchEvalCS2", "SwitchEval", _templateCompiler);
    }

    [Test]
    public void Conditionals()
    {
      AssertRender("ConditionalsCS2", "Conditionals", _templateCompiler);
    }

    [Test]
    public void MultiLine()
    {
      AssertRender("MultiLineCS2", "MultiLine", _templateCompiler);
    }

    [Test]
    public void NullAttributes()
    {
      AssertRender("NullAttributesCS2", "NullAttributes", _templateCompiler);
    }

    [Test]
    public void Partials()
    {
      AssertRender("PartialsCS2", "Partials", _templateCompiler);
    }

    [Test]
    public void Preamble()
    {
      AssertRender("PreambleCS2", "Preamble", _templateCompiler);
    }

    [Test]
    public void Welcome()
    {
      AssertRender("WelcomeCS2", "Welcome", _templateCompiler);
    }

    [Test]
    public void Layout()
    {
      AssertRender("WelcomeCS2", "Application");
    }
  }
}