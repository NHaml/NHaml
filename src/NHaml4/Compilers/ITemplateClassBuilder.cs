using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHaml4.Compilers
{
    public interface ITemplateClassBuilder
    {
        void AppendLine(string content);
        string Build(string className);
    }
}
