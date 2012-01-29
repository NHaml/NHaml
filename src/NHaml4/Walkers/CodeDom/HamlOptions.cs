using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHaml4.Walkers.CodeDom
{
    public enum HtmlVersion
    {
        Html4, Html5, XHtml
    }

    public class HamlOptions
    {
        public HamlOptions()
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
            HtmlVersion = CodeDom.HtmlVersion.XHtml;
        }

        internal bool IsAutoClosingTag(string tagName)
        {
            return AutoClosingTags.Contains(tagName);
        }

        private readonly IList<string> AutoClosingTags;
        public HtmlVersion HtmlVersion;
    }
}
