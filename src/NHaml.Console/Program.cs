using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHaml.Core.Parser;
using NHaml.Core.Visitors;

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

                var wf = new WebFormsVisitor();
                wf.Visit(document);
                System.Console.WriteLine(wf.Result());
            }
            else
            {
                System.Console.WriteLine("Usage: NHaml.Console <input.haml>");
            }
        }
    }
}
