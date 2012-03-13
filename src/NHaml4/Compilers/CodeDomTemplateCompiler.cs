using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NHaml4.Parser;
using System;
using NHaml4.Compilers.Abstract;

namespace NHaml4.Compilers
{
    public class CodeDomTemplateCompiler : ITemplateFactoryCompiler
    {
        private readonly ITemplateTypeBuilder _typeBuilder;

        public CodeDomTemplateCompiler(ITemplateTypeBuilder typeBuilder)
        {
            _typeBuilder = typeBuilder;
        }

        public TemplateFactory Compile(string templateSource, string className, IEnumerable<string> referencedAssemblyLocations)
        {
            var fullAssemblyList = MergeInDefaultCompileTypes(referencedAssemblyLocations);
            var templateType = _typeBuilder.Build(templateSource, className, fullAssemblyList);
            return new TemplateFactory( templateType );
        }

        private IList<string> MergeInDefaultCompileTypes(IEnumerable<string> referencedAssemblyLocations)
        {
            var result = new List<string>(referencedAssemblyLocations);
            if (result.Contains(typeof(TemplateBase.Template).Assembly.Location) == false)
                result.Add(typeof(TemplateBase.Template).Assembly.Location);

            if (result.Contains(typeof(System.Web.HttpUtility).Assembly.Location) == false)
                result.Add(typeof(System.Web.HttpUtility).Assembly.Location);

            return result;
        }
    }
}