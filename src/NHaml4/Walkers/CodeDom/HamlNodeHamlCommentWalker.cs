using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHaml4.Compilers;

namespace NHaml4.Walkers.CodeDom
{
    public class HamlNodeHamlCommentWalker : HamlNodeWalker
    {
        public HamlNodeHamlCommentWalker(ITemplateClassBuilder classBuilder, HamlHtmlOptions options)
            : base(classBuilder, options)
        { }

        public override void Walk(Parser.HamlNode node)
        {
            return;
        }
    }
}
