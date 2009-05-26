using NHaml.Rules;

namespace NHaml.Web.MonoRail
{
    public class ComponentSectionExtension : IMarkupExtension
    {
        public string Name
        {
            get { return "Section"; }
        }

		public BlockClosingAction Render(TemplateParser templateParser, string normalizedSuffix)
        {
			var sectionName = normalizedSuffix;
            var code = string.Format("{0}).AddSection(\"{1}\", x =>", templateParser.CurrentInputLine.Indent, sectionName);

            templateParser.TemplateClassBuilder.EndCodeBlock();
            templateParser.TemplateClassBuilder.Indent();

            templateParser.TemplateClassBuilder.AppendSilentCode(code, false);
            templateParser.TemplateClassBuilder.BeginCodeBlock();


            return templateParser.TemplateClassBuilder.Unindent;
        }

    }
}