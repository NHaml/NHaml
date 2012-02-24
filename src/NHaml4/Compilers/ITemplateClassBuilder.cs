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
        void AppendCode(string codeSnippet);
        void AppendVariable(string variableName);
        string Build(string className, Type baseType);
        void AppendAttributeNameValuePair(string name, IEnumerable<string> valueFragments, char quoteToUse);
        void AppendSelfClosingTagSuffix();
    }
}
