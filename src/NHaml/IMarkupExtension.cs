namespace NHaml
{
    public interface IMarkupExtension
    {
        string Name { get; }

        BlockClosingAction Render(TemplateParser templateParser, string normalizedSuffix);
    }
}