using NUnit.Framework;

namespace NHaml.Tests
{
  public class FunctionalTester : TestFixtureBase
  {
    [Test]
    public void AttributeEval()
    {
      AssertRender("AttributeEval", "AttributeEval");
    }

    [Test]
    public void NullAttributes()
    {
      AssertRender("NullAttributes", "NullAttributes");
    }

    [Test]
    public void SharedPartial()
    {
      AssertRender("SharedPartial", "SharedPartial");
    }

    [Test]
    public void Partials2()
    {
      AssertRender("Partials2", "Partials2");
    }

    [Test]
    public void Partials()
    {
      AssertRender("Partials", "Partials");
    }

    [Test]
    public void Layout()
    {
      AssertRender("Welcome", "Application");
    }

    [Test]
    public void Welcome()
    {
      AssertRender("Welcome");
    }

    [Test]
    public void Javascript()
    {
      AssertRender("Javascript");
    }

    [Test]
    public void Doctype()
    {
      AssertRender("Doctype");
    }

    [Test]
    public void Escape()
    {
      AssertRender("Escape");
    }

    [Test]
    public void Empty()
    {
      AssertRender("Empty");
    }

    [Test]
    public void AutoClose()
    {
      AssertRender("AutoClose");
    }

    [Test]
    public void ReferenceExample1()
    {
      AssertRender("ReferenceExample1");
    }

    [Test]
    public void ReferenceExample2()
    {
      AssertRender("ReferenceExample2");
    }

    [Test]
    public void VeryBasic()
    {
      AssertRender("VeryBasic");
    }

    [Test]
    public void List()
    {
      AssertRender("List");
    }

    [Test]
    public void TagParsing()
    {
      AssertRender("TagParsing");
    }

    [Test]
    public void OriginalEngine()
    {
      AssertRender("OriginalEngine");
    }

    [Test]
    public void SimpleEval()
    {
      AssertRender("SimpleEval");
    }

    [Test]
    public void SilentEval()
    {
      AssertRender("SilentEval");
    }

    [Test]
    public void LoopEval()
    {
      AssertRender("LoopEval");
    }

    [Test]
    public void SwitchEval()
    {
      AssertRender("SwitchEval");
    }

    [Test]
    public void Conditionals()
    {
      AssertRender("Conditionals");
    }

    [Test]
    public void MultiLine()
    {
      AssertRender("MultiLine");
    }

    [Test]
    public void Comments()
    {
      AssertRender("Comments");
    }

    [Test]
    public void AltControllerPartial()
    {
      AssertRender("AltControllerPartial");
    }

    [Test]
    public void WhitespaceSensitive()
    {
      AssertRender("WhitespaceSensitive");
    }
  }
}