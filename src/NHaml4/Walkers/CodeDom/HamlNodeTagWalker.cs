using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHaml4.Compilers;
using NHaml4.Parser;

namespace NHaml4.Walkers.CodeDom
{
    public class HamlNodeTagWalker : INodeWalker
    {
        public void Walk(HamlNode node, ITemplateClassBuilder classBuilder)
        {
            var nodeTag = node as HamlNodeTag;
            if (nodeTag == null)
                throw new InvalidCastException("HamlNodeTagWalker requires that HamlNode object be of type HamlNodeTag.");

            string attributes = GetAttributes(nodeTag.Attributes);
            classBuilder.AppendFormat("<{0}{1}></{0}>", nodeTag.TagName, attributes);
        }

        private string GetAttributes(IEnumerable<KeyValuePair<string, string>> attributes)
        {
            if (attributes.Count() == 0) return "";

            return " " + string.Join(" ", 
                attributes.Select(x => string.Format("{0}='{1}'", x.Key, x.Value)).ToArray());
        }
    }
}
