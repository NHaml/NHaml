using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHaml4.Compilers
{
    public interface ITemplateClassBuilder
    {
        void Append(string content);
        void AppendNewLine();
        string Build(string className);
    }
}
