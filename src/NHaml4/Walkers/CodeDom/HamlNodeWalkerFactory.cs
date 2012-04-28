using System;
using NHaml4.Parser.Rules;
using NHaml4.Compilers;
using NHaml4.Walkers.Exceptions;

namespace NHaml4.Walkers.CodeDom
{
    public static class HamlWalkerFactory
    {
        public static HamlNodeWalker GetNodeWalker(Type nodeType, int sourceFileLineNo, ITemplateClassBuilder classBuilder, HamlHtmlOptions options)
        {
            if (nodeType == typeof(HamlNodeTagId)
                || nodeType == typeof(HamlNodeTagClass)
                || nodeType == typeof(HamlNodeHtmlAttributeCollection)) return null;
            if (nodeType == typeof(HamlNodeTextContainer))
                return new HamlNodeTextContainerWalker(classBuilder, options);
            if (nodeType == typeof(HamlNodeTag))
                return new HamlNodeTagWalker(classBuilder, options);
            if (nodeType == typeof(HamlNodeHtmlComment))
                return new HamlNodeHtmlCommentWalker(classBuilder, options);
            if (nodeType == typeof(HamlNodeHamlComment))
                return new HamlNodeHamlCommentWalker(classBuilder, options);
            if (nodeType == typeof(HamlNodeEval))
                return new HamlNodeEvalWalker(classBuilder, options);
            if (nodeType == typeof(HamlNodeCode))
                return new HamlNodeCodeWalker(classBuilder, options);
            if (nodeType == typeof(HamlNodeTextLiteral))
                return new HamlNodeTextLiteralWalker(classBuilder, options);
            if (nodeType == typeof(HamlNodeTextVariable))
                return new HamlNodeTextVariableWalker(classBuilder, options);
            if (nodeType == typeof(HamlNodeDocType))
                return new HamlNodeDocTypeWalker(classBuilder, options);
            if (nodeType == typeof(HamlNodePartial))
                return new HamlPartialWalker(classBuilder, options);
            
            throw new HamlUnknownNodeTypeException(nodeType, sourceFileLineNo);
        }
    }
}
