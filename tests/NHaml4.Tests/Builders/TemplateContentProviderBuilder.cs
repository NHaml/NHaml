using NHaml4.TemplateResolution;
using Moq;

namespace NHaml.Tests.Builders
{
    public static class TemplateContentProviderBuilder
    {
        public static ITemplateContentProvider Create()
        {
            return Create(ViewSourceBuilder.Create());
        }
        public static ITemplateContentProvider Create(IViewSource fakeHamlSource)
        {
            var stubTemplateContentProvider = new Mock<ITemplateContentProvider>();
            stubTemplateContentProvider.Setup(x => x.GetViewSource(It.IsAny<string>()))
                .Returns(fakeHamlSource);
            return stubTemplateContentProvider.Object;
        }
    }
}
