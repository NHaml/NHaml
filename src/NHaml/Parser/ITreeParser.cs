using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHaml.Parser
{
    public interface ITreeParser
    {
        HamlTree Parse(IList<TemplateResolution.IViewSource> layoutViewSources);
    }
}
