using System.Collections.Generic;
using System;
using System.Reflection.Emit;

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

        private IEnumerable<string> MergeInDefaultCompileTypes(IEnumerable<string> referencedAssemblyLocations)
        {
            var result = new List<string>(referencedAssemblyLocations);
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly is AssemblyBuilder)
                    continue;

                string location;
                try
                {
                    location = assembly.Location;
                }
                catch (NotSupportedException)
                {
                    continue;
                }
                if (result.Contains(location) == false)
                    result.Add(location);
            }
            if (result.Contains(typeof(TemplateBase.Template).Assembly.Location) == false)
                result.Add(typeof(TemplateBase.Template).Assembly.Location);

            if (result.Contains(typeof(System.Web.HttpUtility).Assembly.Location) == false)
                result.Add(typeof(System.Web.HttpUtility).Assembly.Location);

            return result;
        }
    }
}