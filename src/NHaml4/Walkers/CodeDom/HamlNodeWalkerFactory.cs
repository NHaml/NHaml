using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHaml4.Parser;
using NHaml4.Parser.Exceptions;
using NHaml4.Parser.Rules;
using NHaml4.Compilers;

namespace NHaml4.Walkers.CodeDom
{
    public static class HamlWalkerFactory
    {
        public static HamlNodeWalker GetNodeWalker(Type nodeType, int sourceFileLineNo, ITemplateClassBuilder classBuilder, HamlOptions options)
        {
            if (nodeType == typeof(HamlNodeTagId)
                || nodeType == typeof(HamlNodeTagClass)
                || nodeType == typeof(HamlNodeHtmlAttributeCollection)) return null;
            else if (nodeType == typeof(HamlNodeText))
                return new HamlNodeTextWalker(classBuilder, options);
            else if (nodeType == typeof(HamlNodeTag))
                return new HamlNodeTagWalker(classBuilder, options);
            else if (nodeType == typeof(HamlNodeHtmlComment))
                return new HamlNodeHtmlCommentWalker(classBuilder, options);
            else if (nodeType == typeof(HamlNodeHamlComment))
                return new HamlNodeHamlCommentWalker(classBuilder, options);
            else if (nodeType == typeof(HamlNodeEval))
                return new HamlNodeEvalWalker(classBuilder, options);
            else
                throw new HamlUnknownRuleException(nodeType.FullName, sourceFileLineNo);
        }
    }
}
