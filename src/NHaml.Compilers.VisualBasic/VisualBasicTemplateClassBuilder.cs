using System.Collections.Generic;
using System.Text;

namespace NHaml.Compilers.VisualBasic
{
    internal  class VisualBasicTemplateClassBuilder : CodeDomClassBuilder
    {
        public VisualBasicTemplateClassBuilder(string className)
            : base(className)
        {
        }

        protected override void RenderEndBlock()
        {
            
        }


    	protected override string Comment
    	{
			get
			{
				return "'";
			}
    	}

    	public override void AppendAttributeTokens(string schema, string name, IList<ExpressionStringToken> values)
        {
            var code = new StringBuilder();
            foreach (var item in values)
            {
                if (item.IsExpression)
                {
                    code.AppendFormat("({0}) + ", item.Value);
                }
                else
                {
                    code.AppendFormat("\"{0}\" + ", item.Value.Replace("\\\"", "\"\""));
                }
            }

            if (code.Length > 3)
            {
                code.Remove(code.Length - 3, 3);
            }

            var format = string.Format("RenderAttributeIfValueNotNull(textWriter, \"{0}\", \"{1}\",{2})", schema, name,
                                       code);
            AppendSilentCode(format, true);
        }
        protected override void RenderBeginBlock()
        {
        }
    }
}