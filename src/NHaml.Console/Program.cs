using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHaml.Core.Parser;
using NHaml.Core.Visitors;
using NHaml.Core.Compilers;

namespace NHaml.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length >= 1)
            {
                var parser = new Parser();
                var document = parser.ParseFile(args[0]);

                /*var wf = new WebFormsVisitor();
                wf.Visit(document);
                System.Console.WriteLine(wf.Result());*/

                var cf = new CSharpCompiler(document);
                System.Console.WriteLine(cf.GetSource());
            }
            else
            {
                System.Console.WriteLine("Usage: NHaml.Console <input.haml>");
            }
        }
    }
}
