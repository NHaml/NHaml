namespace NHaml.Rules
{
	public interface IMarkupExtension
	{
		string Signifier { get; }

		BlockClosingAction Render(TemplateParser templateParser, string normalizedSuffix);
	}
}