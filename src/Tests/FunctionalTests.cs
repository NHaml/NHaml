using System.IO;

using NUnit.Framework;

namespace NHaml.Tests
{
  public class FunctionalTests : TestFixtureBase
  {
    public override void SetUp()
    {
      base.SetUp();

      _templateCompiler.ViewBaseType = typeof(MockView);
    }

    [Test]
    public void LambdaEval()
    {
      AssertRender("LambdaEval");
    }

    [Test]
    public void AttributeEval()
    {
      AssertRender("AttributeEval");
    }

    [Test]
    public void NullAttributes()
    {
      AssertRender("NullAttributes");
    }

    [Test]
    public void SharedPartial()
    {
      AssertRender("SharedPartial");
    }

    [Test]
    public void Partials2()
    {
      AssertRender("Partials2");
    }

    [Test]
    public void Partials()
    {
      AssertRender("Partials");
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

    public abstract class MockView : CompiledTemplate
    {
      private Html _html;

      public override void Render(TextWriter output)
      {
        _html = new Html(Output);
      }

      public Html Html
      {
        get { return _html; }
      }
    }

    public class Html
    {
      private readonly TemplateOutputWriter _output;

      public Html(TemplateOutputWriter output)
      {
        _output = output;
      }

      public delegate void Action();

      public delegate void Action<T1>(T1 t);

      public delegate void Action<T1, T2>(T1 t1, T2 t2);

      public void Tag(Action action)
      {
        Tag("div", action);
      }

      public void Tag(string name, Action yield)
      {
        _output.WriteLine("<" + name + ">");
        yield();
        _output.WriteLine("</" + name + ">");
      }

      public void Tag<T>(string name, T t, Action<T> yield)
      {
        _output.WriteLine("<" + name + ">");
        yield(t);
        _output.WriteLine("</" + name + ">");
      }

      public void Tag<T1, T2>(string name, T1 t1, T2 t2, Action<T1, T2> yield)
      {
        _output.WriteLine("<" + name + ">");
        yield(t1, t2);
        _output.WriteLine("</" + name + ">");
      }
    }
  }
}