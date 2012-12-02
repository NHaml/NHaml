using NHaml.Parser;
using System;
using System.Collections.Generic;

namespace NHaml.Walkers
{
    public interface IDocumentWalker
    {
        string Walk(HamlDocument hamlDocument, string className, Type baseType, IEnumerable<string> imports);
    }
}
