using System;
using System.CodeDom;
using Microsoft.CSharp;

namespace NHaml.Compilers.CSharp2
{
    internal sealed class CSharp2TemplateClassBuilder : CodeDomClassBuilder
    {
        public CSharp2TemplateClassBuilder(string className, Type baseType) : base(className, baseType, new CSharpCodeProvider())
        {
        }

        protected override void RenderEndBlock()
        {
            RenderMethod.Statements.Add(new CodeSnippetStatement
            {
                Value = "}",
            });
        }

        protected override void RenderBeginBlock()
        {
            RenderMethod.Statements.Add(new CodeSnippetStatement
            {
                Value = "{",
            });
        }
    }
}