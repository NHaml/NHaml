using NHaml4.Parser;

namespace NHaml4.Walkers
{
    public interface IDocumentWalker
    {
        string Walk(HamlDocument hamlDocument, string className);
    }
}
