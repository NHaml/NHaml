namespace NHaml4.Walkers
{
    public interface IWalker
    {
        string ParseHamlDocument(NHaml.Parser.HamlDocument hamlDocument);
    }
}
