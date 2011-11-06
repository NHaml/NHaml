using System;
using System.Collections.Generic;
using System.Text;

namespace NHaml.Core.Ast
{
    public class LateBindingNode : AstNode
    {
        public object Value { get; set; }
    }
}
