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
        public CodeDomProvider CodeDomProvider { get; set; }
        public string Source { get; protected set; }
        public CompilerResults CompilerResults { get; private set; }
        public Dictionary<string, string> ProviderOptions { get; private set; }

        [SuppressMessage( "Microsoft.Security", "CA2122" )]
        public CodeDomTemplateTypeBuilder( TemplateEngine templateEngine )
        {
            ProviderOptions = new Dictionary<string, string>();
            TemplateEngine = templateEngine;
            TemplateEngine.Options.AddReference( GetType().Assembly );

        }


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
                var directoryInfo = GetNHamlTempDirectoryInfo();
                var classFileInfo = GetClassFileInfo(directoryInfo, typeName);
                using (var writer = classFileInfo.CreateText())
                {
                    writer.Write(Source);
                }

                //TODO: when we move to vs2010 fully this ebcomes redundant as it will load the debug info for a  in memory assembly.
                var tempFileName = Path.GetTempFileName();
                var tempAssemblyName = new FileInfo(Path.Combine(directoryInfo.FullName, tempFileName + ".dll"));
                var tempSymbolsName = new FileInfo(Path.Combine(directoryInfo.FullName, tempFileName + ".pdb"));
                try
                {
                    compilerParams.OutputAssembly = tempAssemblyName.FullName;
                    CompilerResults = CodeDomProvider.CompileAssemblyFromFile(compilerParams, classFileInfo.FullName);
                    if (ContainsErrors())
                    {
                        return null;
                    }

                    var assembly = Assembly.Load(File.ReadAllBytes(tempAssemblyName.FullName), File.ReadAllBytes(tempSymbolsName.FullName));
                    return assembly.GetType(typeName);
                }
                finally
                {
                    if (tempAssemblyName.Exists)
                    {
                        tempAssemblyName.Delete();
                    }
                    if (tempSymbolsName.Exists)
                    {
                        tempSymbolsName.Delete();
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

        private FileInfo GetClassFileInfo(DirectoryInfo directoryInfo, string typeName)
        {
            var fileInfo = new FileInfo(string.Format("{0}\\{1}.{2}", directoryInfo.FullName, typeName, CodeDomProvider.FileExtension));
            if (fileInfo.Exists)
            {
                fileInfo.Delete();
            }
            return fileInfo;
        }

        private static DirectoryInfo GetNHamlTempDirectoryInfo()
        {
            var codeBase = Assembly.GetExecutingAssembly().GetName().CodeBase.Remove(0, 8);
            var nhamlTempPath = Path.Combine(Path.GetDirectoryName(codeBase), "nhamlTemp");
            var directoryInfo = new DirectoryInfo(nhamlTempPath);
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }
            return directoryInfo;
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