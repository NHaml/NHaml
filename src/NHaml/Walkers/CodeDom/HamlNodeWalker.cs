using System;
using System.Linq;
using NHaml.Compilers;
using NHaml.Crosscutting;
using NHaml.Parser;
using NHaml.Walkers.Exceptions;

namespace NHaml.Walkers.CodeDom
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
                if (nodeWalker == null) continue;

                try
                {
                    nodeWalker.Walk(child);
                }
                catch (Exception e)
                {
                    throw new HamlNodeWalkerException(child.GetType().Name,
                        child.SourceFileLineNum, e);
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
