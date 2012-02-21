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

    }
}
