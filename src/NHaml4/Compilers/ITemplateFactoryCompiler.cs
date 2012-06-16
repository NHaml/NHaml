using System.Collections.Generic;

namespace NHaml4.Compilers
{
    public interface ITemplateFactoryCompiler
    {
        TemplateFactory Compile(string templateCode, string className, IEnumerable<string> referencedAssemblyLocations);
    }
}