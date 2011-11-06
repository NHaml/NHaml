using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.CodeDom;
using System.CodeDom.Compiler;
using NHaml.Core.Template;
using NHaml.Core.Visitors;
using NHaml.Core.Ast;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace NHaml.Core.Compilers
{
    public abstract class CodeDomVisitorClassBuilder : IClassBuilder
    {
        private DocumentNode _node;

        protected abstract CodeDomProvider Provider { get; }
        protected abstract CodeDomVisitor Visitor { get; }
        protected abstract CompilerParameters CompilerOptions { get; }
        protected abstract CodeGeneratorOptions GeneratorOptions { get; }

        protected virtual CodeCompileUnit GetCompileUnit(TemplateOptions options)
        {
            var compileUnit = new CodeCompileUnit();
            var testNamespace = new CodeNamespace();
            compileUnit.Namespaces.Add(testNamespace);

            foreach (var import in options.Usings)
            {
                var namespaceImport = new CodeNamespaceImport(import);
                testNamespace.Imports.Add(namespaceImport);
            }

            var declaration = new CodeTypeDeclaration
            {
                Name = ClassName,
                IsClass = true,
            };

            CodeMemberMethod[] ctm = new CodeMemberMethod[Visitor.Methods.Count];
            Visitor.Methods.Values.CopyTo(ctm, 0);
            declaration.BaseTypes.Add(options.TemplateBaseType);
            declaration.Members.AddRange(ctm);
            declaration.Members.Add(Visitor.RunContentMethod);
            declaration.Members.Add(Visitor.ContainsContentMethod);

            testNamespace.Types.Add(declaration);
            return compileUnit;
        }

        public virtual void SetDocument(TemplateOptions options, DocumentNode node, string className)
        {
            if (options != null)
            {
                Visitor.Options = options;
            }
            _node = node;
            Visitor.Visit(node);
            ClassName = className;
        }

        public virtual string ClassName { get; protected set; }

        public virtual CompilerResults CompilerResults
        {
            get;
            protected set;
        }

        public virtual TemplateFactory Compile(TemplateOptions options)
        {
            return new TemplateFactory(GenerateType(options));
        }

        public virtual Type GenerateType(TemplateOptions options)
        {
            AddReferences(options);
            if (options.OutputDebugFiles && SupportsDebug())
            {
                CompilerOptions.GenerateInMemory = false;
                CompilerOptions.IncludeDebugInformation = true;
                var directoryInfo = GetNHamlTempDirectoryInfo();
                var classFileInfo = GetClassFileInfo(directoryInfo, ClassName);
                using (var writer = classFileInfo.CreateText())
                {
                    writer.Write(GenerateSource(options));
                }

                //TODO: when we move to vs2010 fully this ebcomes redundant as it will load the debug info for an in memory assembly.
                var tempFileName = Path.GetTempFileName();
                var tempAssemblyName = new FileInfo(Path.Combine(directoryInfo.FullName, tempFileName + ".dll"));
                var tempSymbolsName = new FileInfo(Path.Combine(directoryInfo.FullName, tempFileName + ".pdb"));
                try
                {
                    CompilerOptions.OutputAssembly = tempAssemblyName.FullName;
                    CompilerResults = Provider.CompileAssemblyFromFile(CompilerOptions, classFileInfo.FullName);
                    if (ContainsErrors())
                    {
                        return null;
                    }

                    var assembly = Assembly.Load(File.ReadAllBytes(tempAssemblyName.FullName), File.ReadAllBytes(tempSymbolsName.FullName));
                    return ExtractType(ClassName, assembly);
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
                CompilerOptions.GenerateInMemory = true;
                CompilerOptions.IncludeDebugInformation = false;
                CodeCompileUnit c = GetCompileUnit(options);
                CompilerResults = Provider.CompileAssemblyFromDom(CompilerOptions,c);
                if (ContainsErrors())
                {
                    return null;
                }
                var assembly = CompilerResults.CompiledAssembly;
                return ExtractType(ClassName, assembly);
            }
        }

        public virtual string GenerateSource(TemplateOptions options)
        {
            CodeCompileUnit c = GetCompileUnit(options);
            StringWriter sw = new StringWriter();
            Provider.GenerateCodeFromCompileUnit(c, sw, GeneratorOptions);
            return sw.ToString();
        }


        [SuppressMessage("Microsoft.Security", "CA2122")]
        protected virtual void AddReferences(TemplateOptions options)
        {
            CompilerOptions.ReferencedAssemblies.Clear();

            foreach (var assembly in options.References)
            {
                CompilerOptions.ReferencedAssemblies.Add(assembly);
            }
        }

        protected abstract bool SupportsDebug();

        private static DirectoryInfo GetNHamlTempDirectoryInfo()
        {
            var codeBase = Assembly.GetExecutingAssembly().GetName().CodeBase.Remove(0, 8);
            var runningFolder = Path.GetDirectoryName(codeBase).Replace(@"\", "_").Replace(":", "");
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

        private FileInfo GetClassFileInfo(DirectoryInfo directoryInfo, string typeName)
        {
            var fileInfo = new FileInfo(string.Format("{0}\\{1}.{2}", directoryInfo.FullName, typeName, Provider.FileExtension));
            if (fileInfo.Exists)
            {
                fileInfo.Delete();
            }
            return fileInfo;
        }

        protected virtual bool ContainsErrors()
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

        protected virtual Type ExtractType(string typeName, Assembly assembly)
        {
            return assembly.GetType(typeName);
        }

        public DocumentNode Document
        {
            get { return _node; }
        }
    }
}