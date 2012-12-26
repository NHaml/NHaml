using System.Collections.Generic;
using System.Web.NHaml.Parser;

namespace System.Web.NHaml.Walkers
{
    public interface IDocumentWalker
    {
        string Walk(HamlDocument hamlDocument, string className, Type baseType, IEnumerable<string> imports);
    }
}
