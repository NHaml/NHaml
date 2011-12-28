using NHaml4.Parser;
using NHaml4.Compilers;

namespace NHaml4.Walkers
{
    public interface INodeWalker
    {
        void Walk(HamlNode hamlDocument);
    }
}
