using NHaml4.TemplateResolution;

namespace NHaml4.Parser
{
    public interface ITreeParser
    {
        HamlDocument ParseViewSource(IViewSource viewSource);
    }
}
