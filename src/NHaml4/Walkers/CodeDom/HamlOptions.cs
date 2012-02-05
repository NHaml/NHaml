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
        private readonly IList<string> AutoClosingTags;

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
            Imports = new List<string> { "System" };
        }

        internal bool IsAutoClosingTag(string tagName)
        {
            return AutoClosingTags.Contains(tagName);
        }
        public IList<string> Imports { get; set; }
        public HtmlVersion HtmlVersion;
    }
}
