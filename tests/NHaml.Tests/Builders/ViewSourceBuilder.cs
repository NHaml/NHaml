using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHaml.TemplateResolution;
using System.IO;
using Moq;
using NHaml.Tests.Mocks;

namespace NHaml.Tests.Builders
{
    public static class ViewSourceBuilder
    {
        public static ViewSource Create()
        {
            return Create("Test");
        }

        public static ViewSource Create(string content, string fileName = @"c:\test.haml")
        {
            var stubViewSource = new StreamViewSource(content, fileName);
            return stubViewSource;
        }
    }
}
