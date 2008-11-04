using System;
using System.CodeDom.Compiler;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;

using Boo.Lang.Compiler;
using Boo.Lang.Compiler.IO;
using Boo.Lang.Compiler.Pipelines;

using CompilerError=Boo.Lang.Compiler.CompilerError;

namespace NHaml.Compilers.Boo
{
  internal sealed class BooTemplateTypeBuilder
  {
    private readonly BooCompiler _booCompiler
      = new BooCompiler();

    private readonly CompilerResults _compilerResult =
      new CompilerResults(new TempFileCollection());

    private readonly TemplateEngine _templateEngine;

    private string _source;

    [SuppressMessage("Microsoft.Security", "CA2122")]
    public BooTemplateTypeBuilder(TemplateEngine templateEngine)
    {
      _templateEngine = templateEngine;

      _booCompiler.Parameters.GenerateInMemory = true;
      _booCompiler.Parameters.Debug = true;
      _booCompiler.Parameters.OutputType = CompilerOutputType.Library;
    }

    public string Source
    {
      get { return _source; }
    }

    public CompilerResults CompilerResults
    {
      get { return _compilerResult; }
    }

    [SuppressMessage("Microsoft.Security", "CA2122")]
    [SuppressMessage("Microsoft.Portability", "CA1903")]
    public Type Build(string source, string typeName)
    {
      BuildSource(source);
      AddReferences();

      _booCompiler.Parameters.Input.Clear();
      _booCompiler.Parameters.Input.Add(new StringInput(typeName, _source));
      _booCompiler.Parameters.Pipeline = new CompileToMemory();

      var context = _booCompiler.Run();

      if (context.Errors.Count == 0)
      {
        return context.GeneratedAssembly.GetType(typeName);
      }

      _compilerResult.Errors.Clear();
      foreach (CompilerError error in context.Errors)
      {
        _compilerResult.Errors.Add(new System.CodeDom.Compiler.CompilerError(
          error.LexicalInfo.FileName,
          error.LexicalInfo.Line,
          error.LexicalInfo.Column,
          error.Code,
          error.Message));
      }

      return null;
    }

    [SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods")]
    private void AddReferences()
    {
      foreach (var assemblyFile in _templateEngine.References)
      {
        var assembly = Assembly.LoadFrom(assemblyFile);

        _booCompiler.Parameters.References.Add(assembly);
      }
    }

    private void BuildSource(string source)
    {
      var sourceBuilder = new StringBuilder();

      foreach (var usingStatement in _templateEngine.Usings)
      {
        sourceBuilder.AppendLine("import " + usingStatement);
      }

      sourceBuilder.AppendLine(source);

      _source = sourceBuilder.ToString();
    }
  }
}