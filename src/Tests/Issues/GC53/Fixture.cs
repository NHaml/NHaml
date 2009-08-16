using System.Diagnostics;
using System.IO;
using System.Threading;
using NUnit.Framework;

namespace NHaml.Tests.Issues.GC53
{
    [TestFixture]
    public class Fixture : TestFixtureMvcBase
    {
        public Fixture()
        {
            ExpectedFolder = TemplatesFolder = @"Issues\GC53\";
        }
       
        [Test]
        public void NotRecompilingWhenPartialChanged()
        {
            AssertView( "Content", "Application", "Output" );
            Debug.WriteLine(Path.GetFullPath(@"Issues\GC53\Shared\_Partial.haml"));
            Thread.Sleep(5);
            using (var message = File.AppendText(@"Issues\GC53\Shared\_Partial.haml"))
            {
                message.Write("x");
                message.Flush();
            }

            Output = new StringWriter();
            AssertView( "Content", "Application", "Output2" );
        }
    }
}