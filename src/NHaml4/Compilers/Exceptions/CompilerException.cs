using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom.Compiler;

namespace NHaml4.Compilers.Exceptions
{
    class CompilerException : Exception
    {
        public CompilerException(System.CodeDom.Compiler.CompilerResults compilerResults)
            : base(GenerateExceptionMessage(compilerResults))
        {
        }

        private static string GenerateExceptionMessage(System.CodeDom.Compiler.CompilerResults compilerResults)
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
