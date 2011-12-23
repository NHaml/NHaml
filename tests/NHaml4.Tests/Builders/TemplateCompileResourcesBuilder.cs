using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHaml.Tests.Builders
{
    public static class TemplateCompileResourcesBuilder
    {
        public static TemplateCompileResources Create()
        {
            return new TemplateCompileResources(typeof(TemplateCompileResources), "c:\\test.haml");
        }
    }
}
