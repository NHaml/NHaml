using NHaml;

namespace NHaml4.Compilers
{
    public interface ITemplateFactoryCompiler
    {
        TemplateFactory Compile(string templateCode);
    }
}