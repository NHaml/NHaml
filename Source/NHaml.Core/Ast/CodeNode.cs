using System;

namespace NHaml.Core.Ast
{
    public class CodeNode : AstNode
    {
        public CodeNode(string code)
        {
            if(code == null)
                throw new ArgumentNullException("code");

            Code = code;
        }

        public string Code { get; set; }
    }
}