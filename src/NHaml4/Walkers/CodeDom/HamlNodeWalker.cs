using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHaml4.Compilers;
using NHaml4.Crosscutting;
using NHaml4.Parser;
using NHaml4.Parser.Rules;
using NHaml4.Parser.Exceptions;
using NHaml4.Walkers.Exceptions;

namespace NHaml4.Walkers.CodeDom
{
    public abstract class HamlNodeWalker
    {
        internal readonly ITemplateClassBuilder ClassBuilder;
        internal readonly HamlHtmlOptions Options;

        protected HamlNodeWalker(ITemplateClassBuilder classBuilder, HamlHtmlOptions options)
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
                var nodeWalker = HamlWalkerFactory.GetNodeWalker(child.GetType(), child.SourceFileLineNum, ClassBuilder, Options);
                if (nodeWalker != null)
                {
                    try
                    {
                        nodeWalker.Walk(child);
                    }
                    catch (Exception e)
                    {
                        throw new HamlNodeWalkerException(child.GetType().Name,
                            child.SourceFileLineNum,
                            e);
                    }
                }
            }
        }


        internal void ValidateThereAreNoChildren(HamlNode node)
        {
            if (node.Children.Any())
                throw new HamlInvalidChildNodeException(node.GetType(), node.Children.First().GetType(),
                    node.SourceFileLineNum);
        }
    }
}
