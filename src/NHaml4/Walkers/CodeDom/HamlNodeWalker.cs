using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHaml4.Compilers;
using NHaml4.Crosscutting;
using NHaml4.Parser;

namespace NHaml4.Walkers.CodeDom
{
    public abstract class HamlNodeWalker
    {
        internal readonly ITemplateClassBuilder _classBuilder;
        internal readonly HamlOptions _options;

        protected HamlNodeWalker(ITemplateClassBuilder classBuilder, HamlOptions options)
        {
            Invariant.ArgumentNotNull(options, "options");
            Invariant.ArgumentNotNull(classBuilder, "classBuilder");

            _classBuilder = classBuilder;
            _options = options;
        }

        //public abstract void Walk(HamlNode child);

        //protected void WalkChildren(HamlNode hamlNode)
        //{
        //    foreach (var child in hamlNode.Children)
        //    {
        //        var nodeWalker = GetNodeWalker(child.GetType());
        //        nodeWalker.Walk(child);
        //    }
        //}


        public virtual void Walk(HamlNode node)
        {
            foreach (var child in node.Children)
            {
                var nodeWalker = GetNodeWalker(child.GetType());
                nodeWalker.Walk(child);
            }
        }

        private HamlNodeWalker GetNodeWalker(Type type)
        {
            if (type == typeof(HamlNodeText))
                return new HamlNodeTextWalker(_classBuilder, _options);
            else if (type == typeof(HamlNodeTag))
                return new HamlNodeTagWalker(_classBuilder, _options);
            else if (type == typeof(HamlNodeHtmlComment))
                return new HamlNodeHtmlCommentWalker(_classBuilder, _options);
            else if (type == typeof(HamlNodeHamlComment))
                return new HamlNodeHamlCommentWalker(_classBuilder, _options);
            else
                throw new HamlUnknownRuleException("");
        }
    }
}
