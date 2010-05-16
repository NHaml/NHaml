using System;

namespace NHaml.Core.Ast
{
    public class CodeBlockNode : AstNode
    {
        public CodeBlockNode(string code)
        {
            if (code == null)
                throw new ArgumentNullException("code");

            Code = code;
        }

        public string Code { get; set; }
        public AstNode Child { get; set; }
    }
}