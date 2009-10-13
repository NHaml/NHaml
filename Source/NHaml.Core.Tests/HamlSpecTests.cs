using System;
using System.Diagnostics;
using System.IO;
using NHaml.Core.Tests.Helpers;
using Xunit;

namespace NHaml.Core.Tests
{
    public class HamlSpecTests
    {
        [HamlSpecTheory("tests.json")]
        public void Test(string fullName, string haml, string html, string format, SpecLocal[] locals)
        {
            var output = new StringWriter();
            var parser = new Core.Parser.Parser();
            var writer = new StringWriter();
            var visitor = new DebugVisitor(writer) { Format = format };

            output.WriteLine("Name: " + fullName);
            foreach(var local in locals)
            {
                output.WriteLine("Var:  " + local.Name + "=" + local.Value);
                visitor.Locals.Add(local.Name, local.Value);
            }
            output.WriteLine("Form: " + format);
            output.WriteLine("Haml: " + haml);
            output.WriteLine("Exp:  " + html.Replace(Environment.NewLine, @"\n"));

            try
            {
                var document = parser.Parse(haml);

                visitor.Visit(document);

                writer.Flush();
                output.WriteLine("Out:  " + writer.ToString().Replace(Environment.NewLine, @"\n"));

                output.Flush();
                Assert.True(writer.ToString().Equals(html), output.ToString());

                Debug.Write(output);
                Debug.WriteLine("------------------------");
            }
            catch(Exception exception)
            {
                throw new Exception(output.ToString(), exception);
            }
        }
    }
}