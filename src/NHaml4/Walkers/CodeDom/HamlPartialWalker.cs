using NHaml4.Compilers;
using NHaml4.Parser;
using System;
using System.Collections.Generic;

namespace NHaml4.Walkers.CodeDom
{
    public class HamlPartialWalker : HamlNodeWalker
    {
                public HamlPartialWalker(ITemplateClassBuilder classBuilder, HamlHtmlOptions htmlOptions)
            : base(classBuilder, htmlOptions)
        { }
    }
}
