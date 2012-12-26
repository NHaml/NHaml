using System.Collections.Generic;

namespace System.Web.NHaml.Compilers
{
    public interface ITemplateFactoryCompiler
    {
        TemplateFactory Compile(string templateCode, string className, IEnumerable<string> referencedAssemblyLocations);
    }
}