using System;
using System.CodeDom.Compiler;

namespace NHaml4.Compilers
{
    public interface ITemplateTypeBuilder
    {
        Type Build(string source, string name);
        CompilerResults CompilerResults { get; }
        string Source { get;  }
    }
}