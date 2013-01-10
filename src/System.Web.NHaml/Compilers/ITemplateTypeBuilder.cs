using System.Collections.Generic;

namespace System.Web.NHaml.Compilers
{
    public interface ITemplateTypeBuilder
    {
        Type Build(string source, string name, IEnumerable<string> referencedAssemblyLocations);
    }
}