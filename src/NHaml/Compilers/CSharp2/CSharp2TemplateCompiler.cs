using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

using NHaml.Compilers.CSharp2.Coco;
using NHaml.Exceptions;
using NHaml.Properties;

namespace NHaml.Compilers.CSharp2
{
  public class CSharp2TemplateCompiler : ITemplateCompiler
  {
    private static readonly Regex LambdaRegex = new Regex(
      @"^(.+)(\(.*\))\s*=>\s*$",
      RegexOptions.Compiled | RegexOptions.Singleline);

    private static readonly Regex _keywordEscaperRegex = new Regex(
      @"((^|\s)(class\s*=))|((^|\s)(for\s*=))",
      RegexOptions.Compiled | RegexOptions.Singleline);

    public TemplateClassBuilder CreateTemplateClassBuilder(string className, Type templateBaseType)
    {
      return new CSharp2TemplateClassBuilder(className, templateBaseType);
    }

    public TemplateFactory Compile(TemplateParser templateParser)
    {
      var templateSource = templateParser.TemplateClassBuilder.Build();

      Console.WriteLine(templateSource);
      Trace.WriteLine(templateSource);

      var typeBuilder = CreateTemplateTypeBuilder(templateParser.TemplateEngine);

      var templateType = typeBuilder.Build(templateSource, templateParser.TemplateClassBuilder.ClassName);

      if (templateType == null)
      {
        TemplateCompilationException.Throw(typeBuilder.CompilerResults,
          typeBuilder.Source, templateParser.TemplatePath);
      }

      return new TemplateFactory(templateType);
    }

    public BlockClosingAction RenderSilentEval(TemplateParser templateParser)
    {
      var code = templateParser.CurrentInputLine.NormalizedText;

      var lambdaMatch = LambdaRegex.Match(code);

      if (!lambdaMatch.Success)
      {
        templateParser.TemplateClassBuilder
          .AppendSilentCode(code, !templateParser.IsBlock);

        if (templateParser.IsBlock)
        {
          templateParser.TemplateClassBuilder.BeginCodeBlock();

          return () => templateParser.TemplateClassBuilder.EndCodeBlock();
        }

        return null;
      }

      var depth = templateParser.CurrentInputLine.IndentSize;

      code = TranslateLambda(code, lambdaMatch);

      templateParser.TemplateClassBuilder.AppendChangeOutputDepth(depth);
      templateParser.TemplateClassBuilder.AppendSilentCode(code, true);

      return () =>
        {
          templateParser.TemplateClassBuilder.AppendChangeOutputDepth(depth);
          templateParser.TemplateClassBuilder.AppendSilentCode("})", true);
        };
    }

    public void RenderAttributes(TemplateParser templateParser, string attributes)
    {
      attributes = _keywordEscaperRegex.Replace(attributes, "@$3$6");

      RenderAttributesCore(templateParser, attributes);
    }

    protected virtual void RenderAttributesCore(TemplateParser templateParser, string attributes)
    {
      var stream = new MemoryStream(Encoding.UTF8.GetBytes("class _ {object " + attributes + ";}"));
      var scanner = new Scanner(stream);
      var parser = new Parser(scanner);

      parser.Parse();

      if (parser.errors.count > 0)
      {
        SyntaxException.Throw(templateParser.CurrentInputLine, Resources.AttributesParseError);
      }

      if (parser.variables.Count > 0)
      {
        AppendAttribute(templateParser, parser.variables[0], null);

        for (var i = 1; i < parser.variables.Count; i++)
        {
          AppendAttribute(templateParser, parser.variables[i], " ");
        }
      }
    }

    private static void AppendAttribute(TemplateParser templateParser, DictionaryEntry variable, string separator)
    {
      templateParser.TemplateClassBuilder.AppendAttributeCode(
        separator + variable.Key.ToString().Replace('_', '-').Replace("@", ""),
        variable.Value.ToString());
    }

    public virtual string TranslateLambda(string codeLine, Match lambdaMatch)
    {
      return codeLine.Substring(0, lambdaMatch.Groups[1].Length - 2)
        + (lambdaMatch.Groups[1].Captures[0].Value.Trim().EndsWith("()", StringComparison.OrdinalIgnoreCase) ? null : ", ")
          + "delegate" + lambdaMatch.Groups[2].Captures[0].Value + "{";
    }

    internal virtual CSharp2TemplateTypeBuilder CreateTemplateTypeBuilder(TemplateEngine templateEngine)
    {
      return new CSharp2TemplateTypeBuilder(templateEngine);
    }
  }
}