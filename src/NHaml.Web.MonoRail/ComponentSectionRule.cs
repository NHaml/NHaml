using NHaml.Rules;

namespace NHaml.Web.MonoRail
{
    public class ComponentSectionRule : MarkupRule
    {
        public override char Signifier
        {
            get { return '$'; }
        }

        public override BlockClosingAction Render(TemplateParser templateParser)
        {
            var sectionName = templateParser.CurrentInputLine.NormalizedText;
            var code = string.Format("{0}).AddSection(\"{1}\", x =>", templateParser.CurrentInputLine.Indent,
                                     sectionName);

            templateParser.TemplateClassBuilder.EndCodeBlock();
            templateParser.TemplateClassBuilder.Indent();

            templateParser.TemplateClassBuilder.AppendSilentCode(code, false);
            templateParser.TemplateClassBuilder.BeginCodeBlock();


            return templateParser.TemplateClassBuilder.Unindent;
        }

    }
}