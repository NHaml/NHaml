namespace NHaml.Rules
{
	public class NamedExtensionRule : MarkupRule
	{
		private readonly TemplateEngine templateEngine;

		public NamedExtensionRule(TemplateEngine templateEngine)
		{
			this.templateEngine = templateEngine;
		}

		public override char Signifier
		{
			get { return '$'; }
		}


		public override BlockClosingAction Render(TemplateParser templateParser)
		{
			var text = templateParser.CurrentInputLine.NormalizedText;
			var indexOfSpace = text.IndexOf(' ');
			var extensionName = text.Substring(0, indexOfSpace);
			var markupExtension = templateEngine.MarkupExtensions[extensionName];
			return markupExtension.Render(templateParser, text.Substring(indexOfSpace+1, text.Length - indexOfSpace -1));
		}
	}
}