using System;
using System.Collections.Generic;

using NHaml.Rules;

using NUnit.Framework;

namespace NHaml.Tests
{
  [TestFixture]
  public class TemplateCompilerTests : TestFixtureBase
  {
    public class ViewBase
    {
      public int Foo
      {
        get { return 9; }
      }
    }

    public class ViewBaseGeneric<T> where T : new()
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

    [Test]
    public void BadSignifier()
    {
      var templateCompiler = new TemplateCompiler();

      var inputLine = new InputLine(Convert.ToChar(128).ToString(), 1);

      Assert.AreSame(NullMarkupRule.Instance, templateCompiler.GetRule(inputLine));
    }

    [Test]
    public void ViewBaseClass()
    {
      var templateCompiler = new TemplateCompiler {ViewBaseType = typeof(ViewBase)};

      AssertRender("CustomBaseClass", templateCompiler);
    }

    [Test]
    public void ViewBaseClassGeneric()
    {
      var templateCompiler = new TemplateCompiler {ViewBaseType = typeof(ViewBaseGeneric<>)};

      AssertRender("CustomBaseClass", templateCompiler, typeof(List<int>));
    }

    [Test]
    public void DuplicateUsings()
    {
      var templateCompiler = new TemplateCompiler();
      templateCompiler.AddUsing("System");

      AssertRender("List");
    }

    [Test]
    public void CollectInputFiles()
    {
      var templateCompiler = new TemplateCompiler();

      var inputFiles = new List<string>();

      templateCompiler.Compile(TemplatesFolder + "Partials.haml",
        TemplatesFolder + "Application.haml", inputFiles);

      Assert.AreEqual(3, inputFiles.Count);
    }

    [Test]
    public void ScriptTagNotAutoClosing()
    {
      var templateCompiler = new TemplateCompiler();

      Assert.IsFalse(templateCompiler.IsAutoClosing("script"), "Script was auto-closed");
    }
  }
}