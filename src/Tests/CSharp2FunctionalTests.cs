using NHaml.Compilers.CSharp2;

using NUnit.Framework;

namespace NHaml.Tests
{
  [TestFixture]
  public class CSharp2FunctionalTests : FunctionalTestFixture
  {
    public override void SetUp()
    {
      base.SetUp();

      _templateEngine.TemplateCompiler = new CSharp2TemplateCompiler();
      _primaryTemplatesFolder = "C#2";
    }
  }
}