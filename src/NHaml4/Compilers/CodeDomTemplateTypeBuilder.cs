using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Linq;

namespace NHaml4.Compilers
{
    public abstract class CodeDomTemplateTypeBuilder : ITemplateTypeBuilder
    {
        private readonly CodeDomProvider _codeDomProvider;
        protected Dictionary<string, string> ProviderOptions { get; set; }

        [SuppressMessage( "Microsoft.Security", "CA2122" )]
        protected CodeDomTemplateTypeBuilder(CodeDomProvider codeDomProvider)
        {
            _codeDomProvider = codeDomProvider;
            ProviderOptions = new Dictionary<string, string>();
        }

        [SuppressMessage("Microsoft.Security", "CA2122")]
        [SuppressMessage("Microsoft.Portability", "CA1903")]
        public Type Build(string source, string typeName, IList<Type> references)
        {
            var compilerParams = new CompilerParameters();
            AddReferences(compilerParams, references);
            if (SupportsDebug())
            {
                compilerParams.GenerateInMemory = false;
                compilerParams.IncludeDebugInformation = true;
                var directoryInfo = GetNHamlTempDirectoryInfo();
                var classFileInfo = GetClassFileInfo(directoryInfo, typeName);
                using (var writer = classFileInfo.CreateText())
                {
                    writer.Write(source);
                }

                //TODO: when we move to vs2010 fully this ebcomes redundant as it will load the debug info for an in memory assembly.
                var tempFileName = Path.GetTempFileName();
                var tempAssemblyName = new FileInfo(Path.Combine(directoryInfo.FullName, tempFileName + ".dll"));
                var tempSymbolsName = new FileInfo(Path.Combine(directoryInfo.FullName, tempFileName + ".pdb"));
                try
                {
                    compilerParams.OutputAssembly = tempAssemblyName.FullName;
                    var compilerResults = _codeDomProvider.CompileAssemblyFromFile(compilerParams, classFileInfo.FullName);
                    if (ContainsErrors(compilerResults))
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
                var compilerResults = _codeDomProvider.CompileAssemblyFromSource(compilerParams, source);
                if (ContainsErrors(compilerResults))
                {
                    return null;
                }
                var assembly = compilerResults.CompiledAssembly;
                return ExtractType(typeName, assembly);
            }

        }

        private void AddReferences(CompilerParameters parameters, IEnumerable<Type> references)
        {
            parameters.ReferencedAssemblies.Clear();

            foreach (var type in references)
            {
                parameters.ReferencedAssemblies.Add(type.Assembly.Location);
            }
        }

        private FileInfo GetClassFileInfo(DirectoryInfo directoryInfo, string typeName)
        {
            var fileInfo = new FileInfo(string.Format("{0}\\{1}.{2}", directoryInfo.FullName, typeName, _codeDomProvider.FileExtension));
            if (fileInfo.Exists)
            {
                fileInfo.Delete();
            }
            return fileInfo;
        }

        private static DirectoryInfo GetNHamlTempDirectoryInfo()
        {
            var codeBase = Assembly.GetExecutingAssembly().GetName().CodeBase.Remove(0, 8);
            var runningFolder = Path.GetDirectoryName(codeBase).Replace(@"\", "_").Replace(":","");
            var nhamlTempPath = Path.Combine(Path.GetTempPath(), "nhamlTemp");
            nhamlTempPath = Path.Combine(nhamlTempPath, runningFolder);
            var directoryInfo = new DirectoryInfo(nhamlTempPath);
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }
            Debug.WriteLine(string.Format("NHaml temp directory is '{0}'.", directoryInfo.FullName));
            return directoryInfo;
        }

        protected abstract bool SupportsDebug();

        protected virtual Type ExtractType(string typeName, Assembly assembly)
        {
            return assembly.GetType(typeName);
        }

        private bool ContainsErrors(CompilerResults results)
        {
            foreach (CompilerError error in results.Errors)
            {
                if (error.IsWarning == false) return true;
            }
            return false;
        }



        //[SuppressMessage( "Microsoft.Security", "CA2122" )]
        //private void AddReferences(CompilerParameters parameters)
        //{
        //    parameters.ReferencedAssemblies.Clear();

        //    foreach( var assembly in Options.References )
        //    {
        //        parameters.ReferencedAssemblies.Add( assembly );
        //    }
        //}
    }
}