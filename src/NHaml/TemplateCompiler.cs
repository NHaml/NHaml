using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;

using NHaml.BackEnds;
using NHaml.BackEnds.CSharp3;
using NHaml.Configuration;
using NHaml.Properties;
using NHaml.Rules;
using NHaml.Utils;

namespace NHaml
{
  public sealed class TemplateCompiler
  {
    private static readonly string[] DefaultAutoClosingTags
      = new[] {"META", "IMG", "LINK", "BR", "HR", "INPUT"};

    private static readonly string[] DefaultReferences
      = new[]
          {
            typeof(INotifyPropertyChanged).Assembly.Location,
            typeof(TemplateCompiler).Assembly.Location
          };

    private static readonly string[] DefaultUsings
      = new[] {"System", "System.IO", "NHaml", "NHaml.Utils"};

    private readonly StringSet _autoClosingTags =
      new StringSet(DefaultAutoClosingTags);

    private readonly MarkupRule[] _markupRules
      = new MarkupRule[128];

    private readonly StringSet _references
      = new StringSet(DefaultReferences);

    private readonly StringSet _usings
      = new StringSet(DefaultUsings);

    private ICompilerBackEnd _compilerBackEnd;

    private Type _viewBaseType
      = typeof(CompiledTemplate);

    public TemplateCompiler()
    {
      AddRule(new EofMarkupRule());
      AddRule(new DocTypeMarkupRule());
      AddRule(new TagMarkupRule());
      AddRule(new ClassMarkupRule());
      AddRule(new IdMarkupRule());
      AddRule(new EvalMarkupRule());
      AddRule(new SilentEvalMarkupRule());
      AddRule(new PreambleMarkupRule());
      AddRule(new CommentMarkupRule());
      AddRule(new EscapeMarkupRule());
      AddRule(new PartialMarkupRule());

      _compilerBackEnd = new CSharp3CompilerBackEnd();

      LoadFromConfiguration();
    }

    public ICompilerBackEnd CompilerBackEnd
    {
      get { return _compilerBackEnd; }
      set
      {
        Invariant.ArgumentNotNull(value, "value");
        _compilerBackEnd = value;
      }
    }

    public bool IsProduction { get; set; }

    public Type ViewBaseType
    {
      get { return _viewBaseType; }
      set
      {
        Invariant.ArgumentNotNull(value, "value");

        if (!typeof(CompiledTemplate).IsAssignableFrom(value))
        {
          throw new InvalidOperationException(Resources.InvalidViewBaseType);
        }

        _viewBaseType = value;
        _usings.Add(_viewBaseType.Namespace);
        _references.Add(_viewBaseType.Assembly.Location);
      }
    }

    public IEnumerable<string> Usings
    {
      get { return _usings; }
    }

    public IEnumerable<string> References
    {
      get { return _references; }
    }

    public void LoadFromConfiguration()
    {
      var section = NHamlSection.Read();

      if (section == null)
      {
        return;
      }

      IsProduction = section.Production;

      // Todo: rebuild configuration
      if (!string.IsNullOrEmpty(section.CompilerBackEnd))
      {
        _compilerBackEnd = section.CreateCompilerBackEnd();
      }

      foreach (var assemblyConfigurationElement in section.Assemblies)
      {
        AddReference(Assembly.Load(assemblyConfigurationElement.Name).Location);
      }

      foreach (var namespaceConfigurationElement in section.Namespaces)
      {
        AddUsing(namespaceConfigurationElement.Name);
      }
    }

    public void AddRule(MarkupRule markupRule)
    {
      Invariant.ArgumentNotNull(markupRule, "markupRule");

      _markupRules[markupRule.Signifier] = markupRule;
    }

    public MarkupRule GetRule(InputLine inputLine)
    {
      Invariant.ArgumentNotNull(inputLine, "line");

      if (inputLine.Signifier >= 128)
      {
        return NullMarkupRule.Instance;
      }

      return _markupRules[inputLine.Signifier] ?? NullMarkupRule.Instance;
    }

    public bool IsAutoClosing(string tag)
    {
      Invariant.ArgumentNotEmpty(tag, "tag");

      return _autoClosingTags.Contains(tag.ToUpperInvariant());
    }

    public void AddUsing(string @namespace)
    {
      Invariant.ArgumentNotEmpty(@namespace, "namespace");

      _usings.Add(@namespace);
    }

    public void AddReference(string assemblyLocation)
    {
      Invariant.ArgumentNotEmpty(assemblyLocation, "assemblyLocation");

      _references.Add(assemblyLocation);
    }

    public void AddReferences(Type type)
    {
      AddReference(type.Assembly.Location);

      if (!type.IsGenericType)
      {
        return;
      }

      foreach (var t in type.GetGenericArguments())
      {
        AddReferences(t);
      }
    }

    public TemplateActivator<CompiledTemplate> Compile(string templatePath, params Type[] genericArguments)
    {
      return Compile<CompiledTemplate>(templatePath, genericArguments);
    }

    [SuppressMessage("Microsoft.Design", "CA1004")]
    public TemplateActivator<TView> Compile<TView>(string templatePath, params Type[] genericArguments)
    {
      return Compile<TView>(templatePath, null, genericArguments);
    }

    public TemplateActivator<CompiledTemplate> Compile(string templatePath, string layoutPath, params Type[] genericArguments)
    {
      return Compile<CompiledTemplate>(templatePath, layoutPath, genericArguments);
    }

    [SuppressMessage("Microsoft.Design", "CA1004")]
    public TemplateActivator<TView> Compile<TView>(string templatePath, string layoutPath, params Type[] genericArguments)
    {
      return Compile<TView>(templatePath, layoutPath, null, genericArguments);
    }

    public TemplateActivator<CompiledTemplate> Compile(string templatePath, string layoutPath,
      ICollection<string> inputFiles, params Type[] genericArguments)
    {
      return Compile<CompiledTemplate>(templatePath, layoutPath, inputFiles, genericArguments);
    }

    [SuppressMessage("Microsoft.Design", "CA1004")]
    public TemplateActivator<TView> Compile<TView>(string templatePath, string layoutPath,
      ICollection<string> inputFiles, params Type[] genericArguments)
    {
      Invariant.ArgumentNotEmpty(templatePath, "templatePath");
      Invariant.FileExists(templatePath);

      if (!string.IsNullOrEmpty(layoutPath))
      {
        Invariant.FileExists(layoutPath);
      }

      foreach (var type in genericArguments)
      {
        AddReferences(type);
      }

      var compilationContext
        = new CompilationContext(
          this,
          _compilerBackEnd.AttributeRenderer,
          _compilerBackEnd.SilentEvalRenderer,
          _compilerBackEnd.CreateTemplateClassBuilder(ViewBaseType, ClassNameProvider.MakeClassName(templatePath), genericArguments),
          templatePath,
          layoutPath);

      Compile(compilationContext);

      if (inputFiles != null)
      {
        compilationContext.CollectInputFiles(inputFiles);
      }

      return CreateFastActivator<TView>(_compilerBackEnd.BuildView(compilationContext));
    }

    private static void Compile(CompilationContext compilationContext)
    {
      while (compilationContext.CurrentNode.Next != null)
      {
        var rule = compilationContext.TemplateCompiler.GetRule(compilationContext.CurrentInputLine);

        if (compilationContext.CurrentInputLine.IsMultiline && rule.MergeMultiline)
        {
          compilationContext.CurrentInputLine.Merge(compilationContext.NextInputLine);
          compilationContext.InputLines.Remove(compilationContext.NextNode);
        }
        else
        {
          rule.Process(compilationContext);
        }
      }

      compilationContext.CloseBlocks();
    }

    private static TemplateActivator<TResult> CreateFastActivator<TResult>(Type type)
    {
      var dynamicMethod = new DynamicMethod("activatefast__", type, null, type);

      var ilGenerator = dynamicMethod.GetILGenerator();
      var constructor = type.GetConstructor(new Type[] {});

      if (constructor == null)
      {
        return null;
      }

      ilGenerator.Emit(OpCodes.Newobj, constructor);
      ilGenerator.Emit(OpCodes.Ret);

      return (TemplateActivator<TResult>)dynamicMethod.CreateDelegate(typeof(TemplateActivator<TResult>));
    }
  }
}