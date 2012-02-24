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
        private IList<string> _referencedAssemblyLocations;

        public CodeDomTemplateCompiler(ITemplateTypeBuilder typeBuilder)
            : this(typeBuilder, new List<string>())
        {}

        public CodeDomTemplateCompiler(ITemplateTypeBuilder typeBuilder, IList<string> referencedAssemblyLocations)
        {
            _typeBuilder = typeBuilder;
            _referencedAssemblyLocations = referencedAssemblyLocations;
            MergeInDefaultCompileTypes();
        }

        private void MergeInDefaultCompileTypes()
        {
            if (_referencedAssemblyLocations.Contains(typeof(TemplateBase.Template).Assembly.Location) == false)
                _referencedAssemblyLocations.Add(typeof(TemplateBase.Template).Assembly.Location);

            if (_referencedAssemblyLocations.Contains(typeof(System.Web.HttpUtility).Assembly.Location) == false)
                _referencedAssemblyLocations.Add(typeof(System.Web.HttpUtility).Assembly.Location);
        }

        public TemplateFactory Compile(string templateSource, string className)
        {
            var templateType = _typeBuilder.Build( templateSource, className, _referencedAssemblyLocations);
            return new TemplateFactory( templateType );
        }
    }
}