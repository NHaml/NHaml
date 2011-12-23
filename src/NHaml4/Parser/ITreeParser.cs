using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHaml4.TemplateResolution;

namespace NHaml4.Parser
{
    public interface ITreeParser
    {
        HamlDocument ParseViewSources(ViewSourceList layoutViewSources);
    }
}
