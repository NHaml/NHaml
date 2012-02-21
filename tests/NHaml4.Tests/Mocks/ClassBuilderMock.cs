using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHaml4.Compilers;
using NHaml4.TemplateBase;

namespace NHaml4.Tests.Mocks
{
    public class ClassBuilderMock : ITemplateClassBuilder
    {
        private StringBuilder _output = new StringBuilder();

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

        public void AppendCode(string codeSnippet)
        {
            throw new NotImplementedException();
        }

        public string AppendVariable(string variableName)
        {
            throw new NotImplementedException();
        }

        public string Build(string className)
        {
            return _output.ToString();
        }


        void ITemplateClassBuilder.AppendVariable(string variableName)
        {
            throw new NotImplementedException();
        }


        public void AppendAttributeNameValuePair(string name, IEnumerable<string> valueFragments, char quoteChar)
        {
            string value = string.Join("", valueFragments);
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
    }
}
