using System.Linq;

using NUnit.Framework;

namespace NHaml.Tests
{
  [TestFixture]
  public class TemplateEngineTests : TestFixtureBase
  {
    [Test]
    public void DuplicateUsings()
    {
      _templateEngine.AddUsing("System");

      Assert.AreEqual(5, _templateEngine.Usings.Count());
    }

    [Test]
    public void TemplatesAreCached()
    {
      var templatePath = TemplatesFolder + @"C#2\AttributeEval.haml";

      var compiledTemplate1 = _templateEngine.Compile(templatePath);
      var compiledTemplate2 = _templateEngine.Compile(templatePath);

      Assert.AreSame(compiledTemplate1, compiledTemplate2);
    }

    [Test]
    public void TemplatesWithLayoutsAreCached()
    {
      var templatePath = TemplatesFolder + @"C#2\Welcome.haml";
      var layoutTemplatePath = TemplatesFolder + @"Application.haml";

      var compiledTemplate1 = _templateEngine.Compile(templatePath, layoutTemplatePath);
      var compiledTemplate2 = _templateEngine.Compile(templatePath, layoutTemplatePath);

      Assert.AreSame(compiledTemplate1, compiledTemplate2);
    }
  }
}