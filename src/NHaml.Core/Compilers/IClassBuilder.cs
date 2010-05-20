using System;
using System.Collections.Generic;
using System.Text;
using NHaml.Core.Template;
using System.CodeDom.Compiler;
using NHaml.Core.Ast;

namespace NHaml.Core.Compilers
{
    public interface IClassBuilder
    {
        void SetDocument(DocumentNode node, string className);

        string ClassName { get; }
        DocumentNode Document { get; }

        CompilerResults CompilerResults { get; }

        TemplateFactory Compile(TemplateOptions options);
        Type GenerateType(TemplateOptions options);
        string GenerateSource(TemplateOptions options);
    }
}
