using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHaml.Core.Parser;
using NHaml.Core.Visitors;
using NHaml.Core.Compilers;
using NHaml.Core.Template;

namespace NHaml.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length >= 1)
            {
                List<string> a = new List<string>(args);
                string format = "asp";
                string filename = null;
                while (a.Count > 0)
                {
                    int remove = 1;
                    switch (a[0])
                    {
                        case "-format":
                            if (a.Count > 1)
                            {
                                format = a[1];
                                remove = 2;
                            }
                            break;
                        default:
                            filename = a[0];
                            break;
                    }
                    a.RemoveRange(0, remove);
                }

                var parser = new Parser();
                var document = parser.ParseFile(filename);

                switch (format)
                {
                    case "asp":
                        {
                            var wf = new WebFormsVisitor();
                            wf.Visit(document);
                            System.Console.WriteLine(wf.Result());
                            break;
                        }
                    case "cs":
                        {
                            var cf = new CSharpClassBuilder();
                            cf.SetDocument(null, document, "TestClass");
                            System.Console.WriteLine(cf.GenerateSource(new TemplateOptions()));
                            break;
                        }
                    default:
                        System.Console.WriteLine("Invalid format.");
                        break;
                }
            }
            else
            {
                System.Console.WriteLine("Usage: NHaml.Console [-format {asp|cs}] <input.haml>");
            }
        }
    }
}
