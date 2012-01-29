using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHaml4.Compilers;
using NHaml4.Crosscutting;
using NHaml4.Parser;
using NHaml4.Parser.Rules;
using NHaml4.Parser.Exceptions;

namespace NHaml4.Walkers.CodeDom
{
    public abstract class HamlNodeWalker
    {
        internal readonly ITemplateClassBuilder ClassBuilder;
        internal readonly HamlOptions Options;

        protected HamlNodeWalker(ITemplateClassBuilder classBuilder, HamlOptions options)
        {
            Invariant.ArgumentNotNull(options, "options");
            Invariant.ArgumentNotNull(classBuilder, "classBuilder");

            ClassBuilder = classBuilder;
            Options = options;
        }
       
        public virtual void Walk(HamlNode node)
        {
            foreach (var child in node.Children)
            {
                var nodeWalker = GetNodeWalker(child);
                if (nodeWalker != null)
                {
                    try
                    {
                        nodeWalker.Walk(child);
                    }
                    catch (Exception e)
                    {
                        throw new HamlNodeWalkerException(child.GetType().Name,
                            child.SourceFileLineNo,
                            e);
                    }
                }
            }
        }

        private HamlNodeWalker GetNodeWalker(HamlNode node)
        {
            var type = node.GetType();
            if (type == typeof(HamlNodeTagId)
                || type == typeof(HamlNodeTagClass)
                || type == typeof(HamlNodeHtmlAttributeCollection)) return null;
            else if (type == typeof(HamlNodeText))
                return new HamlNodeTextWalker(ClassBuilder, Options);
            else if (type == typeof(HamlNodeTag))
                return new HamlNodeTagWalker(ClassBuilder, Options);
            else if (type == typeof(HamlNodeHtmlComment))
                return new HamlNodeHtmlCommentWalker(ClassBuilder, Options);
            else if (type == typeof(HamlNodeHamlComment))
                return new HamlNodeHamlCommentWalker(ClassBuilder, Options);
            else
                throw new HamlUnknownRuleException(type.FullName, node.SourceFileLineNo);
        }
    }
}
