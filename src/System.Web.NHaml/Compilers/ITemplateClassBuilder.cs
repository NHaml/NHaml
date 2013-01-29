using System.Collections.Generic;
using System.Web.NHaml.Parser;

namespace System.Web.NHaml.Compilers
{
    public interface ITemplateClassBuilder
    {
        void Append(string content);
        void AppendFormat(string content, params object[] args);
        void AppendNewLine();
        void AppendCodeToString(string codeSnippet);
        void AppendCodeSnippet(string codeSnippet, bool containsChildren);
        void RenderEndBlock();
        void AppendVariable(string variableName);
        void Clear();
        string Build(string className, Type baseType, IEnumerable<string> imports);
        void AppendAttributeNameValuePair(string name, IList<HamlNode> valueFragments, char quoteToUse);
        void AppendSelfClosingTagSuffix();
        void AppendDocType(string docTypeId);
    }
}
