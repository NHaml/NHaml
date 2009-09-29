using NHaml.Core.Parser;
using Xunit;

namespace NHaml.Core.Tests.Parser
{
    public class InputLineTests
    {
        [Fact]
        public void ConstructedRight()
        {
            var line = new InputLine("<line>", 11, 22, true);

            Assert.Equal("<line>",line.Text);
            Assert.Equal(11,line.LineNumber);
            Assert.Equal(22,line.Indent);
            Assert.Equal(true,line.IsMultiline);
        }
    }
}