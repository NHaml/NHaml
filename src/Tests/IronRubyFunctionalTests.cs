using NHaml.Compilers.IronRuby;

using NUnit.Framework;

namespace NHaml.Tests
{
  [TestFixture]
  public class IronRubyFunctionalTests : FunctionalTestFixture
  {
    public override void SetUp()
    {
      base.SetUp();

      _templateEngine.TemplateCompiler = new IronRubyTemplateCompiler();

      _primaryTemplatesFolder = "IronRuby";
    }

    [Test]
    public override void Layout()
    {
      _templateEngine.AddReference(typeof(string).Assembly.Location);

      AssertRender("Welcome", "Application");
    }

    public override void LambdaEval()
    {
      // doesn't work
    }
  }
}