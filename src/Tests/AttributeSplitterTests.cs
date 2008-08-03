using System.IO;
using System.Text;

using NHaml.Coco;

using NUnit.Framework;

namespace NHaml.Tests
{
  [TestFixture]
  public class AttributeSplitterTests
  {
    [Test]
    public void SplitSimple()
    {
      var input =
        "class _ {object @class=\"foo\", http_equiv=10-9, @for=\"baz\"+\"bob\", accept_charset=string.Join(\"-\", new string[]{\"1\",\"2\",\"3\"});}";

      var scanner = new Scanner(new MemoryStream(Encoding.UTF8.GetBytes(input)));
      var parser = new Parser(scanner);

      parser.Parse();

      Assert.AreEqual(0, parser.errors.count);
      Assert.AreEqual(4, parser.variables.Count);

      Assert.AreEqual("@class", parser.variables[0].Key);
      Assert.AreEqual("http_equiv", parser.variables[1].Key);
      Assert.AreEqual("@for", parser.variables[2].Key);
      Assert.AreEqual("accept_charset", parser.variables[3].Key);

      Assert.AreEqual("\"foo\"", parser.variables[0].Value);
      Assert.AreEqual("10-9", parser.variables[1].Value);
      Assert.AreEqual("\"baz\"+\"bob\"", parser.variables[2].Value);
      Assert.AreEqual("string.Join(\"-\", new string[]{\"1\",\"2\",\"3\"})", parser.variables[3].Value);
    }
  }
}