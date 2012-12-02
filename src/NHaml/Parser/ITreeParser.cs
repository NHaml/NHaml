using NHaml.TemplateResolution;

namespace NHaml.Parser
{
    public interface ITreeParser
    {
        HamlDocument ParseViewSource(ViewSource viewSource);
    }
}
