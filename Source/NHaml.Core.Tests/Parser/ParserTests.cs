using System.IO;
using NHaml.Core.Parser;
using Xunit;

namespace NHaml.Core.Tests.Parser
{
    public class ParserTests
    {
        [Fact]
        public void DummyTests()
        {
            var parser = new Core.Parser.Parser();
            var document = parser.Parse("%p#id.class test\n  #test test");

            new ConsoleVisitor().Visit(document);
        }   
    }
}