using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

using Microsoft.CSharp;

namespace NHaml
{
  public sealed class TemplateTypeBuilder
  {
    private readonly Dictionary<string, string> _providerOptions
      = new Dictionary<string, string>();

    private readonly TemplateCompiler _templateCompiler;

    private readonly CompilerParameters _compilerParameters
      = new CompilerParameters();

    private CompilerResults _compilerResults;

    private string _source;

    [SuppressMessage("Microsoft.Security", "CA2122")]
    public TemplateTypeBuilder(TemplateCompiler templateCompiler)
    {
      _templateCompiler = templateCompiler;

      _providerOptions.Add("CompilerVersion", "v3.5");

      _compilerParameters.GenerateInMemory = true;
      _compilerParameters.IncludeDebugInformation = false;
    }

    public string Source
    {
      get { return _source; }
    }

    public CompilerResults CompilerResults
    {
      get { return _compilerResults; }
    }

    public CompilerParameters CompilerParameters
    {
      get { return _compilerParameters; }
    }

    [SuppressMessage("Microsoft.Security", "CA2122")]
    public Type Build(string source, string typeName)
    {
      BuildSource(source);
      AddReferences();

      var codeProvider = new CSharpCodeProvider(_providerOptions);

      _compilerResults = codeProvider
        .CompileAssemblyFromSource(_compilerParameters, _source);

      if (_compilerResults.Errors.Count == 0)
      {
        return _compilerResults.CompiledAssembly.GetType(typeName);
      }

      return null;
    }

    [SuppressMessage("Microsoft.Security", "CA2122")]
    private void AddReferences()
    {
      _compilerParameters.ReferencedAssemblies.Clear();

      foreach (string assembly in _templateCompiler.References)
      {
        _compilerParameters.ReferencedAssemblies.Add(assembly);
      }
    }

    private void BuildSource(string source)
    {
      var sourceBuilder = new StringBuilder();

      foreach (string usingStatement in _templateCompiler.Usings)
      {
        sourceBuilder.AppendLine("using " + usingStatement + ";");
      }

      sourceBuilder.AppendLine(source);

      _source = sourceBuilder.ToString();
    }
  }
}