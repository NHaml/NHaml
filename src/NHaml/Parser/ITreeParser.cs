using System.Web.NHaml.TemplateResolution;

namespace System.Web.NHaml.Parser
{
    public interface ITreeParser
    {
        HamlDocument ParseViewSource(ViewSource viewSource);
    }
}
