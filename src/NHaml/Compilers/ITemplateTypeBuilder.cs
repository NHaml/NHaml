using System;
using System.CodeDom.Compiler;

namespace NHaml.Compilers
{
    public interface ITemplateTypeBuilder
    {
        Type Build(string source, string name);
        CompilerResults CompilerResults { get; }
        string Source { get;  }
    }
}