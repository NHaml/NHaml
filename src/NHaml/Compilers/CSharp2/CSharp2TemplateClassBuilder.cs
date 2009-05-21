using System;
using System.CodeDom;

namespace NHaml.Compilers.CSharp2
{
    internal sealed class CSharp2TemplateClassBuilder : CodeDomClassBuilder
    {
        public CSharp2TemplateClassBuilder(string className, Type baseType) : base(className, baseType)
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