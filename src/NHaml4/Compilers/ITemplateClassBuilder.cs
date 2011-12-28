using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHaml4.Compilers
{
    public interface ITemplateClassBuilder
    {
        void Append(string content);
        void AppendFormat(string content, params object[] args);
        void AppendNewLine();
        string Build(string className);
    }
}
