using System.Web.NHaml.Compilers;

namespace System.Web.NHaml.Walkers.CodeDom
{
    public class HamlPartialWalker : HamlNodeWalker
    {
        public HamlPartialWalker(ITemplateClassBuilder classBuilder, HamlHtmlOptions htmlOptions)
            : base(classBuilder, htmlOptions)
        { }
    }
}
