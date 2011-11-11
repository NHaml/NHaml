using System.CodeDom;

namespace NHaml.Compilers.CSharp2
{
    internal class CSharp2TemplateClassBuilder : CodeDomClassBuilder
    {
        public CSharp2TemplateClassBuilder(string className) : base(className)
        {
        }

    	protected override string Comment
    	{
			get
			{
				return "//";
			}
    	}

    	protected override void RenderEndBlock()
        {
			RenderMethod.Statements.Add(new CodeSnippetExpression
            {
                Value = "}//",
            });
        }

        protected override void RenderBeginBlock()
        {
            RenderMethod.Statements.Add(new CodeSnippetExpression
            {
				Value = "{//",
            });
        }
    }
}