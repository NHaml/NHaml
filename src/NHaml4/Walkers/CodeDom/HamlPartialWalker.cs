using NHaml4.Compilers;

namespace NHaml4.Walkers.CodeDom
{
    public class HamlPartialWalker : HamlNodeWalker
    {
        public HamlPartialWalker(ITemplateClassBuilder classBuilder, HamlHtmlOptions htmlOptions)
            : base(classBuilder, htmlOptions)
        { }
    }
}
