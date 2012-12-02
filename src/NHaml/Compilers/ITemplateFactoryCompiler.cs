using System.Collections.Generic;

namespace NHaml.Compilers
{
    public interface ITemplateFactoryCompiler
    {
        TemplateFactory Compile(string templateCode, string className, IEnumerable<string> referencedAssemblyLocations);
    }
}