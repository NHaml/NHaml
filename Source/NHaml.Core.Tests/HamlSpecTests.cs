using System;
using System.Diagnostics;
using System.IO;
using NHaml.Core.Tests.Helpers;
using NHaml.Core.Tests.Parser;
using Xunit;

namespace NHaml.Core.Tests
{
    public class HamlSpecTests
    {
        [HamlSpecTheory]
        public void Test(string fullName, string haml, string html, string format)
        {
            if(!string.IsNullOrEmpty(format)||
                haml.Contains("(")||
                haml.Contains("=>")||
                haml.Contains("#{"))
                return;

            var output = new StringWriter();
            output.WriteLine("Name: " + fullName);
            output.WriteLine("Form: " + format);
            output.WriteLine("Haml: " + haml);
            output.WriteLine("Exp:  " + html.Replace(Environment.NewLine, @"\n"));

            try
            {
                var parser = new Core.Parser.Parser();

                var document = parser.Parse(haml);

                var writer = new StringWriter();

                new DebugVisitor(writer).Visit(document);

                writer.Flush();
                output.WriteLine("Out:  " + writer.ToString().Replace(Environment.NewLine,@"\n"));
                
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