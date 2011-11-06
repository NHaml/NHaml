using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHaml.TemplateResolution;
using System.IO;
using Moq;

namespace NHaml.Tests.Builders
{
    public static class ViewSourceBuilder
    {
        public static IViewSource Create()
        {
            return Create("Test");
        }

        internal static IViewSource Create(string content)
        {
            var streamReader = new StreamReader(new MemoryStream(new System.Text.UTF8Encoding().GetBytes(content)   ));
            var stubViewSource = new Mock<IViewSource>();
            stubViewSource.Setup(x => x.GetStreamReader()).Returns(streamReader);

            return stubViewSource.Object;
        }
    }
}
