using System;
using System.IO;
using NHaml4.Crosscutting;
using System.Collections.Generic;

namespace NHaml4.TemplateBase
{
    public abstract class Template
    {
        private IDictionary<string, object> _viewData;
        public IDictionary<string, object> ViewData
        {
            get { return _viewData; }
        }

        private HtmlVersion _htmlVersion;
        protected bool HasCodeBlockRepeated;

        public void Render(TextWriter writer)
        {
            Render(writer, HtmlVersion.XHtml, new Dictionary<string, object>());
        }

        public void Render(TextWriter writer, HtmlVersion htmlVersion)
        {
            Render(writer, htmlVersion, new Dictionary<string, object>());
        }

        public void Render(TextWriter writer, HtmlVersion htmlVersion, IDictionary<string, object> viewData)
        {
            Invariant.ArgumentNotNull(writer, "textWriter");
            _htmlVersion = htmlVersion;
            _viewData = viewData;
            HasCodeBlockRepeated = false;
            CoreRender(writer);
        }

        protected virtual void CoreRender(TextWriter textWriter)
        {
        }

        protected string RenderValueOrKeyAsString(string keyName)
        {
            return !string.IsNullOrEmpty(keyName) && _viewData.ContainsKey(keyName)
                ? Convert.ToString(_viewData[keyName])
                : keyName;
        }

        protected string RenderAttributeNameValuePair(string name, string value, char quoteToUse)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(value) || value.ToLower() == "false")
                return "";
            else if (value.ToLower() == "true" || string.IsNullOrEmpty(value))
                return _htmlVersion == HtmlVersion.XHtml
                    ? " " + name + "=" + quoteToUse + name + quoteToUse
                    : " " + name;
            else
                return " " + name + "=" + quoteToUse + value + quoteToUse;
        }

        public string AppendSelfClosingTagSuffix()
        {
            return _htmlVersion == HtmlVersion.XHtml ? " />" : ">";
        }

        public void SetViewData(IDictionary<string, object> viewData)
        {
            _viewData = viewData;
        }

        public void SetHtmlVersion(HtmlVersion htmlVersion)
        {
            _htmlVersion = htmlVersion;
        }

        public void WriteNewLineIfRepeated(TextWriter writer)
        {
            if (HasCodeBlockRepeated) writer.WriteLine();
            HasCodeBlockRepeated = true;
        }
    }
}
