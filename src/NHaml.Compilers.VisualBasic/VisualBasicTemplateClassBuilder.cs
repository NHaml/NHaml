using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualBasic;

namespace NHaml.Compilers.VisualBasic
{
    internal sealed class VisualBasicTemplateClassBuilder : CodeDomClassBuilder
    {
        public VisualBasicTemplateClassBuilder(string className, Type baseType)
            : base(className, baseType, new VBCodeProvider())
        {
        }

        protected override void RenderEndBlock()
        {
            
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