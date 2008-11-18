using System;
using System.Collections.Generic;
using System.IO;

using NUnit.Framework;

namespace NHaml.Tests
{
  [TestFixture]
  public abstract class FunctionalTestFixture : TestFixtureBase
  {
    [Test]
    [ExpectedException(typeof(FileNotFoundException))]
    public void BadPartial()
    {
      AssertRender("BadPartial");
    }

    [Test]
    public void Tabs()
    {
      _templateEngine.UseTabs = true;

      AssertRender("Tabs");
    }

    [Test]
    public void FourSpaces()
    {
      _templateEngine.IndentSize = 4;

      AssertRender("4Spaces");
    }

//    [Test]
//    public void Alligators()
//    {
//      AssertRender("Alligators");
//    }

    [Test]
    public void VeryBasic()
    {
      AssertRender("VeryBasic");
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
    public void EscapeHtmlOffByDefault()
    {
      AssertRender("EscapeHtml", null, "EscapeHtmlOff");
    }

    [Test]
    public void EscapeHtmlOnByDefault()
    {
      _templateEngine.EncodeHtml = true;

      AssertRender("EscapeHtml", null, "EscapeHtmlOn");
    }

    [Test]
    public virtual void Empty()
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
    public void Comments()
    {
      AssertRender("Comments");
    }

    [Test]
    public void WhitespaceSensitive()
    {
      AssertRender("WhitespaceSensitive");
    }

    public class CustomHelper
    {
      private readonly OutputWriter _output;

      public CustomHelper(OutputWriter output)
      {
        _output = output;
      }

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

    [Test]
    public virtual void LambdaEval()
    {
      _templateEngine.TemplateBaseType = typeof(CustomTemplate1);
      _templateEngine.AddReference(typeof(Action).Assembly.Location);

      AssertRender("LambdaEval");
    }

    [Test]
    public void AttributeEval()
    {
      AssertRender("AttributeEval");
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
    public virtual void SwitchEval()
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
    public void NullAttributes()
    {
      AssertRender("NullAttributes");
    }

    [Test]
    public void Partials()
    {
      AssertRender("Partials");
    }

    [Test]
    public void Preamble()
    {
      AssertRender("Preamble");
    }

    [Test]
    public void Welcome()
    {
      AssertRender("Welcome");
    }

    [Test]
    public virtual void Layout()
    {
      AssertRender("Welcome", "Application");
    }

    [Test]
    [ExpectedException(typeof(InvalidOperationException))]
    public void LayoutNoContent()
    {
      AssertRender("Application");
    }

    [Test]
    public void ViewBaseClass()
    {
      _templateEngine.TemplateBaseType = typeof(CustomTemplate2);

      AssertRender("CustomBaseClass");
    }

    [Test]
    public void ViewBaseClassGeneric()
    {
      _templateEngine.TemplateBaseType = typeof(CustomTemplate3<List<int>>);

      AssertRender("CustomBaseClass");
    }

    [Test]
    public virtual void ViewBaseClassGeneric2()
    {
      _templateEngine.TemplateBaseType = typeof(CustomTemplate4<List<List<int>>>);

      AssertRender("CustomBaseClass");
    }
  }

  public abstract class CustomTemplate1 : Template
  {
    private FunctionalTestFixture.CustomHelper _customHelper;

    protected override void PreRender(OutputWriter outputWriter)
    {
      _customHelper = new FunctionalTestFixture.CustomHelper(outputWriter);
    }

    public FunctionalTestFixture.CustomHelper Html
    {
      get { return _customHelper; }
    }
  }

  public abstract class CustomTemplate2 : Template
  {
    public int Foo
    {
      get { return 9; }
    }
  }

  public abstract class CustomTemplate3<T> : Template
    where T : new()
  {
    public int Foo
    {
      get
      {
        object o = new T();

        var list = (List<int>)o;

        return list.Count + 9;
      }
    }
  }

  public abstract class CustomTemplate4<T> : Template
    where T : new()
  {
    public int Foo
    {
      get
      {
        object o = new T();

        var list = (List<List<int>>)o;

        return list.Count + 9;
      }
    }
  }
}