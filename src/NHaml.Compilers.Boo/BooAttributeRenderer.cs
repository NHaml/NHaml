using System;
using System.Collections.Generic;

using antlr;

using Boo.Lang.Compiler.Ast;
using Boo.Lang.Parser;

using NHaml.Compilers.Boo.Properties;
using NHaml.Exceptions;
using NHaml.Utils;

namespace NHaml.Compilers.Boo
{
    internal static class BooAttributeRenderer
    {
        public static void Render( TemplateParser templateParser, string attributes )
        {
            var errorList = new List<RecognitionException>();

            var expression = BooParser.ParseExpression( 2, "attributes", "{" + attributes + "}", errorList.Add );

            foreach( var exception in errorList )
            {
                SyntaxException.Throw( templateParser.CurrentInputLine,
                  Resources.AttributesParse_BooParserError, exception.Message );
            }

            var hashLiteralExpression = expression as HashLiteralExpression;

            if( hashLiteralExpression != null )
            {
                Render( templateParser, hashLiteralExpression );
            }
            else
            {
                var blockExpression = expression as BlockExpression;

                if( blockExpression != null )
                {
                    Render( templateParser, blockExpression );
                }
                else
                {
                    SyntaxException.Throw(
                      templateParser.CurrentInputLine,
                      Utility.FormatCurrentCulture( Resources.AttributesParse_UnexpectedExpression, expression.GetType().FullName ) );
                }
            }
        }

        private static void Render( TemplateParser templateParser, BlockExpression blockExpression )
        {
            if( blockExpression.Body == null ||
              blockExpression.Body.Statements.Count == 0 )
            {
                return;
            }

            if( blockExpression.Body.Statements.Count > 1 )
            {
                SyntaxException.Throw( templateParser.CurrentInputLine,
                  Resources.AttributesParse_StatementGreaterThenOne );
            }

            var statement = blockExpression.Body.Statements[0];

            var expressionStatement = statement as ExpressionStatement;

            if( expressionStatement == null )
            {
                var type = statement == null ? "null" : statement.GetType().FullName;

                SyntaxException.Throw( templateParser.CurrentInputLine,
                  Utility.FormatCurrentCulture( Resources.AttributesParse_ExpressionStatementExpected, type ) );
            }

            var expression = expressionStatement != null ? expressionStatement.Expression : null;

            var binaryExpression = expression as BinaryExpression;

            if( binaryExpression != null )
            {
                AppendAttribute( templateParser, binaryExpression.Left, binaryExpression.Right, false );

                return;
            }

            var listLiteralExpression = expression as ListLiteralExpression;

            if( listLiteralExpression != null )
            {
                Render( templateParser, listLiteralExpression );

                return;
            }

            var statementType = statement == null ? "null" : statement.GetType().FullName;

            SyntaxException.Throw( templateParser.CurrentInputLine,
              Utility.FormatCurrentCulture(
                Resources.AttributesParse_BinaryExpressionExpected, statementType ) );
        }

        private static void Render( TemplateParser templateParser, ListLiteralExpression listLiteralExpression )
        {
            var first = true;
            foreach( var item in listLiteralExpression.Items )
            {
                var binaryExpression = item as BinaryExpression;

                if( binaryExpression != null )
                {
                    AppendAttribute( templateParser, binaryExpression.Left, binaryExpression.Right, !first );

                    return;
                }

                var statementType = item == null ? "null" : item.GetType().FullName;

                SyntaxException.Throw( templateParser.CurrentInputLine,
                  Utility.FormatCurrentCulture(
                    Resources.AttributesParse_BinaryExpressionExpected, statementType ) );

                first = false;
            }
        }

        private static void Render( TemplateParser templateParser, HashLiteralExpression hashExpression )
        {
            var first = true;
            foreach( var item in hashExpression.Items )
            {
                AppendAttribute( templateParser, item.First, item.Second, !first );
                first = false;
            }
        }

        private static void AppendAttribute( TemplateParser templateParser, Expression key, Node value, bool useSeparator )
        {
            if( key is LiteralExpression )
            {
                var literal = (StringLiteralExpression)key;

                AppendAttribute( templateParser, literal.Value, value.ToCodeString(), useSeparator );
            }
            else
            {
                var referenceExpression = key as ReferenceExpression;

                if( referenceExpression != null )
                {
                    AppendAttribute( templateParser, referenceExpression.Name, value.ToCodeString(), useSeparator );
                }
                else
                {
                    var message = Utility.FormatCurrentCulture(
                      Resources.AttributesParse_UnexpectedAttributeExpression, key.GetType().FullName );

                    SyntaxException.Throw( templateParser.CurrentInputLine, message );
                }
            }
        }

        private static void AppendAttribute( TemplateParser templateParser, string key, string value, bool useSeparator )
        {
            templateParser.TemplateClassBuilder.AppendAttributeCode(
              (useSeparator ? " " : String.Empty) + key.Replace( '_', '-' ).Replace( "@", "" ),
              value );
        }
    }
}