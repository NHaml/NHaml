using System;
using System.IO;

using NUnit.Framework;

namespace NHaml.Tests
{
  public abstract class TestFixtureBase
  {
    protected const string TemplatesFolder = @"Templates\";
    protected const string ExpectedFolder = @"Expected\";

    protected TemplateEngine _templateEngine;

    protected string _primaryTemplatesFolder;
    protected string _secondaryTemplatesFolder;

    [SetUp]
    public virtual void SetUp()
    {
      _templateEngine = new TemplateEngine();
    }

    protected void AssertRender(string templateName)
    {
      AssertRender(templateName, null);
    }

    protected void AssertRender(string templateName, string layoutName)
    {
      var expected = templateName;

      var templatePath = TemplatesFolder + _primaryTemplatesFolder + "\\" + templateName + ".haml";

      if (!File.Exists(templatePath))
      {
        templatePath = TemplatesFolder + _secondaryTemplatesFolder + "\\" + templateName + ".haml";
      }

      if (!File.Exists(templatePath))
      {
        templatePath = TemplatesFolder + templateName + ".haml";
      }

      if (!string.IsNullOrEmpty(layoutName))
      {
        expected = layoutName;
        layoutName = TemplatesFolder + layoutName + ".haml";
      }

      var compiledTemplate = _templateEngine.Compile(templatePath, layoutName);
      var template = compiledTemplate.CreateInstance();

      var output = new StringWriter();

      template.Render(output);

      Console.WriteLine(output);

      Assert.AreEqual(File.ReadAllText(ExpectedFolder + expected + ".xhtml"), output.ToString());
    }
  }
}