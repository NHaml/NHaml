using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NHaml.TemplateResolution;
using System.IO;

namespace NHaml.Tests.Builders
{
    public static class TemplateOptionsBuilder
    {
        internal static TemplateOptions Create()
        {
            return new TemplateOptions
            {
                TemplateContentProvider = GetStubTemplateContentProvider().Object
            };
        }

        private static Mock<ITemplateContentProvider> GetStubTemplateContentProvider()
        {
            var stubTemplateContentProvider = new Mock<ITemplateContentProvider>();
            stubTemplateContentProvider.Setup(x => x.GetViewSource(It.IsAny<string>())).Returns(
                ViewSourceBuilder.Create());
            return stubTemplateContentProvider;
        }
    }
}
