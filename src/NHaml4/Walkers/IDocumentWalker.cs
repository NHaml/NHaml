using NHaml4.Parser;
using System;
using System.Collections.Generic;

namespace NHaml4.Walkers
{
    public interface IDocumentWalker
    {
        string Walk(HamlDocument hamlDocument, string className, Type baseType, IEnumerable<string> imports);
    }
}
