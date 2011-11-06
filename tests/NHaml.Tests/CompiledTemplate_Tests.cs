using NUnit.Framework;
using NHaml.Parser;
using Moq;
using NHaml.Tests.Builders;

namespace NHaml.Tests
{
    [TestFixture]
    public class CompiledTemplate_Tests
    {
        [Test]
        public void CompileTemplateFactory_CallsTreeParser()
        {
            var templateOptions = TemplateOptionsBuilder.Create();
            var templateCompileResources = TemplateCompileResourcesBuilder.Create();
            var parserMock = new Mock<ITreeParser>();

            var compiledTemplate = new CompiledTemplate(templateOptions, templateCompileResources, parserMock.Object);
            compiledTemplate.CompileTemplateFactory();

            parserMock.Verify(x => x.Parse(templateCompileResources.GetViewSources(templateOptions.TemplateContentProvider)));
        }
    }
}