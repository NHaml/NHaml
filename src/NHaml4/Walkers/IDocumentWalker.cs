using NHaml4.Parser;
using System;

namespace NHaml4.Walkers
{
    public interface IDocumentWalker
    {
        string Walk(HamlDocument hamlDocument, string className, Type baseType);
    }
}
