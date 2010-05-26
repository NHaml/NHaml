using System;

namespace NHaml.Core.Ast
{
    public class CodeNode : AstNode
    {
        public CodeNode(string code, bool? escape)
        {
            if(code == null)
                throw new ArgumentNullException("code");

            Code = code;
            Escape = escape;
        }

        public string Code { get; set; }
        public bool? Escape { get; set; }
    }
}