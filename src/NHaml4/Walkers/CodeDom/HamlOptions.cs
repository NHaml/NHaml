using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHaml4.Walkers.CodeDom
{
    public class HamlHtmlOptions
    {
        private readonly IList<string> AutoClosingTags;

        public HamlHtmlOptions()
        {
            AutoClosingTags = new List<string> {
                                    "area",
                                    "base",
                                    "br",
                                    "col",
                                    "hr",
                                    "img",
                                    "input",
                                    "link",
                                    "meta",
                                    "param"
                                };
        }

        internal bool IsAutoClosingTag(string tagName)
        {
            return AutoClosingTags.Contains(tagName);
        }
    }
}
