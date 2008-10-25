using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using antlr;

using Boo.Lang.Compiler.Ast;
using Boo.Lang.Parser;

using NHaml.BackEnds.Boo.Properties;
using NHaml.Exceptions;
using NHaml.Utils;

namespace NHaml.BackEnds.Boo
{
  [SuppressMessage("Microsoft.Naming", "CA1722")]
  public sealed class BooAttributeRenderer : IAttributeRenderer
  {
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

      var hashLiteralExpression = expression as HashLiteralExpression;

      if (hashLiteralExpression != null)
      {
        Render(compilationContext, hashLiteralExpression);
      }
      else
      {
        var blockExpression = expression as BlockExpression;

        if (blockExpression != null)
        {
          Render(compilationContext, blockExpression);
        }
        else
        {
          SyntaxException.Throw(
            compilationContext.CurrentInputLine,
            Utility.FormatCurrentCulture(Resources.AttributesParse_UnexpectedExpression, expression.GetType().FullName));
        }
      }
    }

    private static void Render(CompilationContext compilationContext, BlockExpression blockExpression)
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

      var expressionStatement = statement as ExpressionStatement;

      if (expressionStatement == null)
      {
        var type = statement == null ? "null" : statement.GetType().FullName;

        SyntaxException.Throw(compilationContext.CurrentInputLine,
          Utility.FormatCurrentCulture(Resources.AttributesParse_ExpressionStatementExpected, type));
      }

// ReSharper disable PossibleNullReferenceException
      var expression = expressionStatement.Expression;
// ReSharper restore PossibleNullReferenceException

      var binaryExpression = expression as BinaryExpression;

      if (binaryExpression == null)
      {
        var type = statement == null ? "null" : statement.GetType().FullName;

        SyntaxException.Throw(compilationContext.CurrentInputLine,
          Utility.FormatCurrentCulture(Resources.AttributesParse_ExpressionStatementExpected, type));
      }

// ReSharper disable PossibleNullReferenceException
      AppendAttribute(compilationContext, binaryExpression.Left, binaryExpression.Right, false);
// ReSharper restore PossibleNullReferenceException
    }

    private static void Render(CompilationContext compilationContext,
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
      Expression key, Node value, bool useSeparator)
    {
      if (key is LiteralExpression)
      {
        var literal = (StringLiteralExpression)key;

        AppendAttribute(compilationContext, literal.Value, value.ToCodeString(), useSeparator);
      }
      else
      {
        var referenceExpression = key as ReferenceExpression;

        if (referenceExpression != null)
        {
          AppendAttribute(compilationContext, referenceExpression.Name, value.ToCodeString(), useSeparator);
        }
        else
        {
          var message = Utility.FormatCurrentCulture(
            Resources.AttributesParse_UnexpectedAttributeExpression, key.GetType().FullName);

          SyntaxException.Throw(compilationContext.CurrentInputLine, message);
        }
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