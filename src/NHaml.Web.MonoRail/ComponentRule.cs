using System.CodeDom;
using System.Collections.Generic;
using NHaml.Compilers;
using NHaml.Rules;

namespace NHaml.Web.MonoRail
{
    public class ComponentRule : MarkupRule
    {
        private static int tempDictionaryCount = 0;

        public override char Signifier
        {
            get { return '^'; }
        }

        protected virtual string PreprocessLine(InputLine inputLine)
        {
            return inputLine.NormalizedText;
        }

        public override BlockClosingAction Render(TemplateParser templateParser)
        {
            var text = templateParser.CurrentInputLine.NormalizedText;

            var dictionary = string.Empty;
            string componentName;
            if (text.Contains("{"))
            {
                var indexOf = text.IndexOf(' ');
                dictionary = text.Substring(indexOf + 2, text.Length - indexOf - 3);
                componentName = text.Substring(0, indexOf);
            }
            else
            {
                componentName = text;
            }


            var builder = (CodeDomClassBuilder)templateParser.TemplateClassBuilder;

            var dictionaryLocalVariable = AppendCreateDictionaryLocalVariable(dictionary, builder);

            var code = string.Format("{0}Component(\"{1}\", {2}, () =>",
                                     templateParser.CurrentInputLine.Indent, componentName, dictionaryLocalVariable);

            builder.AppendSilentCode(code, false);
            builder.BeginCodeBlock();
            return () =>
                       {
                           builder.EndCodeBlock();
                           builder.AppendSilentCode(").Render();", false);
                       };
        }

        private static string AppendCreateDictionaryLocalVariable(string dictionary, CodeDomClassBuilder builder)
        {
            var parser = new AttributeParser(dictionary);
            parser.Parse();


            var keyValuePairType = new CodeTypeReference(typeof(KeyValuePair<string,object>));

            var createDictionaryMethod = new CodeMethodInvokeExpression();


            foreach (var attribute in parser.Attributes)
            {

                var newKeyValueExpression = new CodeObjectCreateExpression {CreateType = keyValuePairType};

                var keyExpression = new CodePrimitiveExpression {Value = attribute.Name};
                newKeyValueExpression.Parameters.Add(keyExpression);

                if (attribute.Type == ParsedAttributeType.String)
                {
                    var expressionStringParser = new ExpressionStringParser(attribute.Value);

                    expressionStringParser.Parse();
                    var values = expressionStringParser.Tokens;
                    if (values.Count == 1)
                    {
                        var expressionStringToken = values[0];
                        if (expressionStringToken.IsExpression)
                        {
                            newKeyValueExpression.Parameters.Add(new CodeSnippetExpression
                                                        {
                                                            Value = expressionStringToken.Value

                                                        });
                        }
                        else
                        {

                            newKeyValueExpression.Parameters.Add(new CodePrimitiveExpression
                                                        {
                                                            Value = expressionStringToken.Value

                                                        });
                        }
                    }
                    else
                    {
                        var concatExpression = CodeDomClassBuilder.GetConcatExpression(values);
                        newKeyValueExpression.Parameters.Add(concatExpression);
                    }
                }
                else
                {
                    newKeyValueExpression.Parameters.Add(new CodeSnippetExpression
                                                             {
                                                                 Value = attribute.Value

                                                             });

                }


                createDictionaryMethod.Parameters.Add(newKeyValueExpression);
            }



            var getDictionaryMethod = new CodeMethodReferenceExpression
                                          {
                                              MethodName = "GetDictionaryFromKeyValue",
                                              TargetObject =
                                                  new CodeVariableReferenceExpression
                                                      {
                                                          VariableName = "NHamlMonoRailView"
                                                      }
                                          };
            createDictionaryMethod.Method = getDictionaryMethod;

            var variableName = "nhamlTempDictionary" + tempDictionaryCount;
            tempDictionaryCount++;
            var _decl1 = new CodeVariableDeclarationStatement
                             {
                                 InitExpression = createDictionaryMethod,
                                 Name = variableName,
                                 Type = new CodeTypeReference(typeof (IDictionary<string, object>))
                             };
            builder.RenderMethod.Statements.Add(_decl1);
            return variableName;
        }

    }
}