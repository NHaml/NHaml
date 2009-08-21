using System.CodeDom;

namespace NHaml.Compilers.CSharp2
{
    internal class CSharp2TemplateClassBuilder : CodeDomClassBuilder
    {
        public CSharp2TemplateClassBuilder(string className) : base(className)
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