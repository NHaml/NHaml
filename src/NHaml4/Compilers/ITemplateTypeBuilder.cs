using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;

namespace NHaml4.Compilers
{
    public interface ITemplateTypeBuilder
    {
        Type Build(string source, string name, IList<Type> references);
        CompilerResults CompilerResults { get; }
    }
}