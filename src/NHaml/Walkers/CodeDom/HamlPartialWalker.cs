using NHaml.Compilers;

namespace NHaml.Walkers.CodeDom
{
    public class HamlPartialWalker : HamlNodeWalker
    {
        public HamlPartialWalker(ITemplateClassBuilder classBuilder, HamlHtmlOptions htmlOptions)
            : base(classBuilder, htmlOptions)
        { }
    }
}
