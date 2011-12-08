using NHaml4.Parser;

namespace NHaml4.Walkers
{
    public interface IHamlTreeWalker
    {
        string Walk(HamlDocument hamlDocument, string className);
    }
}
