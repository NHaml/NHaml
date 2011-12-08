using System.CodeDom;

namespace NHaml4.Compilers.CSharp2
{
    public class CSharp2TemplateClassBuilder : CodeDomClassBuilder
    {
        public CSharp2TemplateClassBuilder() : base()
        {
        }

    	protected override string CommentMarkup
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