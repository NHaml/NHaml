using NHaml.Compilers;
using NHaml.Rules;

namespace NHaml.Web.MonoRail
{
    public class ComponentSectionMarkupRule : MarkupRule
    {
        public override BlockClosingAction Render(ViewSourceReader viewSourceReader, TemplateOptions options, TemplateClassBuilder builder)
        {
            var text = viewSourceReader.CurrentInputLine.NormalizedText.TrimStart();
		    var indexOfSpace = text.IndexOf(' ');
            var sectionName = text.Substring(indexOfSpace + 1, text.Length - indexOfSpace - 1);
            var code = string.Format("{0}).AddSection(\"{1}\", x =>", viewSourceReader.CurrentInputLine.Indent, sectionName);

            builder.EndCodeBlock();
            builder.Indent();

            builder.AppendSilentCode(code, false);
            builder.BeginCodeBlock();


            return builder.Unindent;
        }

        public override string Signifier
        {
            get { return "$Section"; }
        }

    
    }
}