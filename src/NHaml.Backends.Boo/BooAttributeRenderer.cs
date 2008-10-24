using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using antlr;

using Boo.Lang.Compiler.Ast;
using Boo.Lang.Parser;

using NHaml.Backends.Boo.Properties;
using NHaml.Exceptions;
using NHaml.Utils;

namespace NHaml.Backends.Boo
{
  [SuppressMessage("Microsoft.Naming", "CA1722")]
  public sealed class BooAttributeRenderer : IAttributeRenderer
  {
    #region IAttributeRenderer Members

    public void Render(CompilationContext compilationContext, string attributes)
    {
      var errorList = new List<RecognitionException>();

      var expression = BooParser.ParseExpression(2, "attributes", "{" + attributes + "}", errorList.Add);

      foreach (var exception in errorList)
      {
        SyntaxException.Throw(compilationContext.CurrentInputLine,
          Utility.FormatCurrentCulture(
            Resources.AttributesParse_BooParserError, exception.Message));
      }

      if (expression is HashLiteralExpression)
      {
        Render(compilationContext, (HashLiteralExpression)expression);
      }
      else if (expression is BlockExpression)
      {
        Render(compilationContext, (BlockExpression)expression);
      }
      else
      {
        SyntaxException.Throw(
          compilationContext.CurrentInputLine,
          Utility.FormatCurrentCulture(Resources.AttributesParse_UnexpectedExpression, expression.GetType().FullName));
      }
    }

    #endregion

    private void Render(CompilationContext compilationContext,
      BlockExpression blockExpression)
    {
      if (blockExpression.Body == null ||
        blockExpression.Body.Statements.Count == 0)
      {
        return;
      }

      if (blockExpression.Body.Statements.Count > 1)
      {
        SyntaxException.Throw(compilationContext.CurrentInputLine,
          Resources.AttributesParse_StatementGreaterThenOne);
      }

      var statement = blockExpression.Body.Statements[0];

      if (!(statement is ExpressionStatement))
      {
        var type = statement == null ? "null" : statement.GetType().FullName;
        SyntaxException.Throw(compilationContext.CurrentInputLine,
          Utility.FormatCurrentCulture(
            Resources.AttributesParse_ExpressionStatementExpected, type));
      }

      var expression = ((ExpressionStatement)statement).Expression;

      if (!(expression is BinaryExpression))
      {
        var type = statement == null ? "null" : statement.GetType().FullName;
        SyntaxException.Throw(compilationContext.CurrentInputLine,
          Utility.FormatCurrentCulture(Resources.AttributesParse_ExpressionStatementExpected, type));
      }

      var binaryExpression = (BinaryExpression)expression;

      AppendAttribute(compilationContext, binaryExpression.Left, binaryExpression.Right, false);
    }

    private void Render(CompilationContext compilationContext,
      HashLiteralExpression hashExpression)
    {
      var first = true;
      foreach (var item in hashExpression.Items)
      {
        AppendAttribute(compilationContext, item.First, item.Second, !first);
        first = false;
      }
    }

    private static void AppendAttribute(CompilationContext compilationContext,
      Expression key, Expression value, bool useSeparator)
    {
      if (key is LiteralExpression)
      {
        var literal = (StringLiteralExpression)key;
        AppendAttribute(compilationContext, literal.Value, value.ToCodeString(), useSeparator);
      }
      else if (key is ReferenceExpression)
      {
        var name = ((ReferenceExpression)key).Name;
        AppendAttribute(compilationContext, name, value.ToCodeString(), useSeparator);
      }
      else
      {
        var message = string.Format(Resources.AttributesParse_UnexpectedAttributeExpression, key.GetType().FullName);
        SyntaxException.Throw(compilationContext.CurrentInputLine, message);
      }
    }

    private static void AppendAttribute(CompilationContext compilationContext,
      string key, string value, bool useSeparator)
    {
      compilationContext.TemplateClassBuilder.AppendAttributeCode(
        (useSeparator ? " " : String.Empty) + key.Replace('_', '-').Replace("@", ""),
        value);
    }
  }
}