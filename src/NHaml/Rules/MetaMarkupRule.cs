
using NHaml.Compilers;
using System.Collections.Generic;

namespace NHaml.Rules
{
    public class MetaMarkupRule : MarkupRule
    {

        public override string Signifier
        {
            get { return "@"; }
        }

        public override BlockClosingAction Render(ViewSourceReader viewSourceReader, TemplateOptions options, TemplateClassBuilder builder)
        {
            var content = viewSourceReader.CurrentInputLine.NormalizedText.Trim().Replace( "\"", "\"\"" );

            var indexOfEquals = content.IndexOf('=');
            var key = content.Substring(0, indexOfEquals).Trim();
            var value = content.Substring(indexOfEquals+1, content.Length - indexOfEquals - 1).Trim();

            if (key == "type")
            {
              key = "model";
            }
            if (!builder.Meta.ContainsKey(key))
            {
              builder.Meta[key] = new List<string>();
            }
            builder.Meta[key].Add(value);

            return EmptyClosingAction;
        }
    }
}