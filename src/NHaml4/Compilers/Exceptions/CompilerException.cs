using System;
using System.CodeDom.Compiler;

namespace NHaml4.Compilers.Exceptions
{
    [Serializable]
    class CompilerException : Exception
    {
        private string _sourceCode;

        public CompilerException(CompilerResults compilerResults, string source)
            : base(GenerateExceptionMessage(compilerResults))
        {
            _sourceCode = source;
        }

        private static string GenerateExceptionMessage(CompilerResults compilerResults)
        {
            string outputMessage = "";

            foreach (CompilerError message in compilerResults.Errors)
            {
                if (message.IsWarning) continue;
                if (!string.IsNullOrEmpty(outputMessage)) outputMessage += Environment.NewLine;
                outputMessage += message.ErrorText + " : Line " + message.Line;
            }

            return outputMessage;
        }

        public string SourceCode
        {
            get { return _sourceCode; }
        }
    }
}
