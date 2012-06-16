using System;
using System.Collections.Generic;

namespace NHaml4.Compilers
{
    public interface ITemplateTypeBuilder
    {
        Type Build(string source, string name, IEnumerable<string> referencedAssemblyLocations);
    }
}