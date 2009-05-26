namespace NHaml.Rules
{
	public interface IMarkupExtension
	{
		string Name { get; }

		BlockClosingAction Render(TemplateParser templateParser, string normalizedSuffix);
	}
}