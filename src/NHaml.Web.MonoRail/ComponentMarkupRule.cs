using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using NHaml.Compilers;
using NHaml.Rules;

namespace NHaml.Web.MonoRail
{
    public class ComponentMarkupRule : MarkupRule
    {
        private static int tempDictionaryCount;

        public override BlockClosingAction Render(ViewSourceReader viewSourceReader, TemplateOptions options, TemplateClassBuilder builder)
        {

            var dictionary = string.Empty;

            var text = viewSourceReader.CurrentInputLine.NormalizedText.Trim();
            var indexOfSpace = text.IndexOf(' ');
            string componentName ;
            if (indexOfSpace == -1)
            {
                componentName = text.Trim();
            }
            else
            {
                dictionary = text.Substring(indexOfSpace, text.Length - indexOfSpace);
                dictionary = dictionary.Trim();
                Debug.Assert(dictionary.StartsWith("{"), "dictionary must start with '{'");
                Debug.Assert(dictionary.EndsWith("}"), "dictionary must start with '}'");
                dictionary = dictionary.Substring(1, dictionary.Length - 2);
                componentName = text.Substring(0, indexOfSpace);
            }


            var codeDomClassBuilder = (CodeDomClassBuilder)builder;

            var dictionaryLocalVariable = AppendCreateDictionaryLocalVariable(dictionary, codeDomClassBuilder);

        	codeDomClassBuilder.CurrentTextWriterVariableName = "x";
            var code = string.Format("{0}Component(\"{1}\", {2}, (x) =>",
                                     viewSourceReader.CurrentInputLine.Indent, componentName, dictionaryLocalVariable);

            codeDomClassBuilder.AppendSilentCode(code, false);
            codeDomClassBuilder.BeginCodeBlock();
			return () =>
			       	{
			       		codeDomClassBuilder.EndCodeBlock();
			       		codeDomClassBuilder.AppendSilentCode(").Render();", false);
			       		codeDomClassBuilder.CurrentTextWriterVariableName = TemplateClassBuilder.DefaultTextWriterVariableName;
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
                    AppendStringDictValue(attribute, newKeyValueExpression);
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

            var variableName = string.Format("nhamlTempDictionary{0}", tempDictionaryCount);
            tempDictionaryCount++;
            var dictionaryDecleration = new CodeVariableDeclarationStatement
                             {
                                 InitExpression = createDictionaryMethod,
                                 Name = variableName,
                                 Type = new CodeTypeReference(typeof (IDictionary<string, object>))
                             };
            builder.RenderMethod.Statements.Add(dictionaryDecleration);
            return variableName;
        }

    	private static void AppendStringDictValue(ParsedAttribute attribute, CodeObjectCreateExpression newKeyValueExpression)
    	{
    		var expressionStringParser = new ExpressionStringParser(attribute.Value);

    		expressionStringParser.Parse();
            var values = expressionStringParser.ExpressionStringTokens;
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

        public override string Signifier
        {
            get { return "$Component"; }
        }

     
    }
}