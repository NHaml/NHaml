using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;

namespace NHaml.Compilers
{
    public abstract class CodeDomTemplateTypeBuilder : ITemplateTypeBuilder
    {

        public TemplateEngine TemplateEngine { get; private set; }

        [SuppressMessage( "Microsoft.Security", "CA2122" )]
        public CodeDomTemplateTypeBuilder( TemplateEngine templateEngine )
        {
            ProviderOptions = new Dictionary<string, string>();
            TemplateEngine = templateEngine;
            TemplateEngine.Options.AddReference( GetType().Assembly );

        }


        public string Source { get; protected set; }

        public CompilerResults CompilerResults { get; private set; }

        public Dictionary<string, string> ProviderOptions { get; private set; }

        [SuppressMessage("Microsoft.Security", "CA2122")]
        [SuppressMessage("Microsoft.Portability", "CA1903")]
        public Type Build(string source, string typeName)
        {
            BuildSource(source);

            Trace.WriteLine(Source);


            var compilerParams = new CompilerParameters();
            AddReferences(compilerParams);
            if (TemplateEngine.Options.OutputDebugFiles && SupportsDebug())
            {
                compilerParams.GenerateInMemory = false;
                compilerParams.IncludeDebugInformation = true;
                var directoryInfo = new DirectoryInfo("nhamlTemp");
                if (!directoryInfo.Exists)
                {
                    directoryInfo.Create();
                }
                var fileInfo = new FileInfo(string.Format("nhamltemp\\{0}.{1}", typeName, CodeDomProvider.FileExtension));
                if (fileInfo.Exists)
                {
                    fileInfo.Delete();
                }
                using (var writer = fileInfo.CreateText())
                {
                    writer.Write(Source);
                }

            //TODO: when we move to vs2010 fully this ebcomes redundant as it will load the debug info for a  in memory assembly.
                var tempFileName = Path.GetTempFileName();

                var tempAssemblyName = Path.Combine(directoryInfo.FullName, tempFileName + ".dll");
                var tempSymbolsName = Path.Combine(directoryInfo.FullName, tempFileName + ".pdb");
                try
                {
                    compilerParams.OutputAssembly = tempAssemblyName;
                    CompilerResults = CodeDomProvider.CompileAssemblyFromFile(compilerParams, fileInfo.FullName);
                    if (ContainsErrors())
                    {
                        return null;
                    }

                    var file = Path.GetFullPath(tempAssemblyName);
                    var evidence = Path.GetFullPath(tempSymbolsName);
                    var assembly = Assembly.Load(File.ReadAllBytes(file), File.ReadAllBytes(evidence));
                    return assembly.GetType(typeName);
                }
                finally 
                {
                    if (File.Exists(tempAssemblyName))
                    {
                        File.Delete(tempAssemblyName);
                    }
                    if (File.Exists(tempSymbolsName))
                    {
                        File.Delete(tempSymbolsName);
                    }
                }
            }
            else
            {
                compilerParams.GenerateInMemory = true;
                compilerParams.IncludeDebugInformation = false;
                CompilerResults = CodeDomProvider.CompileAssemblyFromSource(compilerParams, Source);
                if (ContainsErrors())
                {
                    return null;
                }
                var assembly = CompilerResults.CompiledAssembly;
                return ExtractType(typeName, assembly);
            }

        }

        protected abstract bool SupportsDebug();

        protected virtual Type ExtractType(string typeName, Assembly assembly)
        {
            return assembly.GetType(typeName);
        }

        private bool ContainsErrors()
        {
            foreach (CompilerError result in CompilerResults.Errors)
            {
                if (!result.IsWarning)
                {
                    return true;
                }
            }
            return false;
        }

        public CodeDomProvider CodeDomProvider { get; set; }


        [SuppressMessage( "Microsoft.Security", "CA2122" )]
        private void AddReferences(CompilerParameters parameters)
        {
            parameters.ReferencedAssemblies.Clear();

            foreach( var assembly in TemplateEngine.Options.References )
            {
                parameters.ReferencedAssemblies.Add( assembly );
            }
        }

        protected virtual void BuildSource( string source )
        {
            Source = source;
        }
    }
}