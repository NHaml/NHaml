using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHaml4.Compilers;

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

        public string Build(string className)
        {
            return _output.ToString();
        }
    }
}
