using System;
using System.IO;

using NUnit.Framework;

namespace NHaml.Tests
{
  [TestFixture]
  public abstract class TestFixtureBase
  {
    protected const string TemplatesFolder = @"Templates\";
    protected const string ResultsFolder = @"Results\";

    protected TemplateCompiler _templateCompiler;

    [SetUp]
    public virtual void SetUp()
    {
      _templateCompiler = new TemplateCompiler();

      Assert.AreEqual("2.0", _templateCompiler.CompilerVersion);

      _templateCompiler.CompilerVersion = "3.5";
    }

    protected void AssertRender(string template)
    {
      AssertRender(template, _templateCompiler);
    }

    protected void AssertRender(string template, string layout)
    {
      var viewActivator = _templateCompiler.Compile(
        TemplatesFolder + template + ".haml",
        TemplatesFolder + layout + ".haml");

      var view = viewActivator();

      var output = new StringWriter();

      view.Render(output);

      //Console.WriteLine(output);

      Assert.AreEqual(File.ReadAllText(ResultsFolder + layout + ".xhtml"), output.ToString());
    }

    protected static void AssertRender(string template, TemplateCompiler templateCompiler,
      params Type[] genericArguments)
    {
      var viewActivator = templateCompiler.Compile(
        TemplatesFolder + template + ".haml", genericArguments);

      var view = viewActivator();

      var output = new StringWriter();

      view.Render(output);

      //Console.WriteLine(output);

      Assert.AreEqual(File.ReadAllText(ResultsFolder + template + ".xhtml"), output.ToString());
    }
  }
}