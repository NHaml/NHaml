namespace NHaml.Rules
{
	public class NamedExtensionRule : MarkupRule
	{
		public override char Signifier
		{
			get { return '$'; }
		}

        public override BlockClosingAction Render(TemplateParser templateParser)
		{
			var text = templateParser.CurrentInputLine.NormalizedText;
			
            var indexOfSpace = text.IndexOf(' ');
			var extensionName = text.Substring(0, indexOfSpace);
            var markupExtension = templateParser.TemplateEngine.Options.GetExtension( extensionName );
			
            return markupExtension.Render(templateParser, text.Substring(indexOfSpace+1, text.Length - indexOfSpace -1));
		}
	}
}