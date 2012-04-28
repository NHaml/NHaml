using System.Collections.Generic;

namespace NHaml4.Walkers.CodeDom
{
    public class HamlHtmlOptions
    {
        private readonly IList<string> _autoClosingTags;

        public HamlHtmlOptions()
        {
            _autoClosingTags = new List<string> {
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
            return _autoClosingTags.Contains(tagName);
        }
    }
}
