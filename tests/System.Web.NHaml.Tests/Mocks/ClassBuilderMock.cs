using System;
using System.Collections.Generic;
using System.Text;
using System.Web.NHaml.Compilers;
using System.Web.NHaml.Parser;
using System.Linq;
using System.Web.NHaml.Parser.Rules;

namespace NHaml.Tests.Mocks
{
    public class ClassBuilderMock : ITemplateClassBuilder
    {
        private readonly StringBuilder _output = new StringBuilder();

        public void Append(string content)
        {
            _output.Append(content);
        }

        public void AppendFormat(string content, params object[] args)
        {
            _output.AppendFormat(content, args);
        }

        public void AppendNewLine()
        {
            _output.AppendLine();
        }

        public void AppendCodeToString(string codeSnippet)
        {
            throw new NotImplementedException();
        }

        public void AppendCodeSnippet(string codeSnippet, bool containsChildren)
        { }

        public void RenderEndBlock()
        { }

        public string AppendVariable(string variableName)
        {
            throw new NotImplementedException();
        }

        public string Build(string className)
        {
            return _output.ToString();
        }

        public string Build(string className, Type baseType, IEnumerable<string> usings)
        {
            return _output.ToString();
        }

        public void Clear()
        {
            _output.Clear();
        }

        void ITemplateClassBuilder.AppendVariable(string variableName)
        {
            throw new NotImplementedException();
        }


        public void AppendAttributeNameValuePair(string name, IEnumerable<HamlNode> valueFragments, char quoteChar)
        {
            string value = string.Join("", valueFragments.Select(x => x is HamlNodeTextVariable ? ((HamlNodeTextVariable)x).VariableName : x.Content));
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(value) || value.ToLower() == "false")
                return;
            else if (value.ToLower() == "true" || string.IsNullOrEmpty(value))
                _output.Append(" " + name + "=" + quoteChar + name + quoteChar);
            else
                _output.Append(" " + name + "=" + quoteChar + value + quoteChar);
        }


        public void AppendSelfClosingTagSuffix()
        {
            _output.Append(" />");
        }


        public void AppendDocType(string docTypeId)
        {
            throw new NotImplementedException();
        }
    }
}
