using NHaml.Rules;

namespace NHaml.Web.MonoRail
{
    public class ComponentSectionMarkupRule : MarkupRule
    {

		public override BlockClosingAction Render(TemplateParser templateParser)
        {
            var text = templateParser.CurrentInputLine.NormalizedText.TrimStart();
		    var indexOfSpace = text.IndexOf(' ');
            var sectionName = text.Substring(indexOfSpace + 1, text.Length - indexOfSpace - 1);
            var code = string.Format("{0}).AddSection(\"{1}\", x =>", templateParser.CurrentInputLine.Indent, sectionName);

            templateParser.TemplateClassBuilder.EndCodeBlock();
            templateParser.TemplateClassBuilder.Indent();

            templateParser.TemplateClassBuilder.AppendSilentCode(code, false);
            templateParser.TemplateClassBuilder.BeginCodeBlock();


            return templateParser.TemplateClassBuilder.Unindent;
        }

        public override string Signifier
        {
            get { return "$Section"; }
        }

    
    }
}