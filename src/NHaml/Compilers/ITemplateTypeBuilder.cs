using System;
using System.Collections.Generic;

namespace NHaml.Compilers
{
    public interface ITemplateTypeBuilder
    {
        Type Build(string source, string name, IEnumerable<string> referencedAssemblyLocations);
    }
}