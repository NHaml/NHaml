using System;
using System.CodeDom.Compiler;

namespace NHaml4.Compilers.Exceptions
{
    class CompilerException : Exception
    {
        public CompilerException(CompilerResults compilerResults)
            : base(GenerateExceptionMessage(compilerResults))
        {
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
    }
}
