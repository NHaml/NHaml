using System.Diagnostics;
using System.IO;
using System.Printing;
using System.Threading;
using NUnit.Framework;

namespace NHaml.Xps.Tests
{
    [TestFixture]
    public class XpsEngineTestFixture
    {
        [Test]
        [Ignore]
        public void PrintPreview()
        {
            var runner = new CrossThreadTestRunner();
            runner.RunInSTA(
                () =>
                {
                    var xpsHelper = new XpsEngine();
                    xpsHelper.PrintPreview("XpsWithData.haml", "Hello");
                });
        }
        [Test]
        [Ignore]
        public void GetQueTimeTest()
        {
            using (var printServer = new LocalPrintServer())
            {
                using (var queue = printServer.GetPrintQueue("Microsoft XPS Document Writer"))
                {
                    Debug.WriteLine(queue.Name);
                }
            }
            var stopwatch = Stopwatch.StartNew();
            using (var printServer2 = new LocalPrintServer())
            {
                using (var queue2 = printServer2.GetPrintQueue("Microsoft XPS Document Writer"))
                {
                    Debug.WriteLine(queue2.Name);
                }
            }
            stopwatch.Start();
            Debug.WriteLine(stopwatch.ElapsedMilliseconds);
        }

        [Test]
        [Ignore]
        public void Print()
        {
            var xpsHelper = new XpsEngine();
            using (var autoResetEvent = new AutoResetEvent(false))
            {
                xpsHelper.Print("XpsWithData.haml", "Hello2", LocalPrintServer.GetDefaultPrintQueue, (sender, e) => autoResetEvent.Set());
                autoResetEvent.WaitOne();
            }
        }

        [Test]
        public void WriteToFile()
        {
            var runner = new CrossThreadTestRunner();
            runner.RunInSTA(
                () =>
                {
                    const string tempTarget = "temp.xps";
                    try
                    {
                        if (File.Exists(tempTarget))
                        {
                            File.Delete(tempTarget);
                        }
                        var xpsHelper = new XpsEngine();
                        xpsHelper.Generate("XpsWithData.haml", "Hello", tempTarget);
                        Assert.IsTrue(File.Exists(tempTarget));
                    }
                    finally
                    {
                        if (File.Exists(tempTarget))
                        {
                            File.Delete(tempTarget);
                        }
                    }
                });
        }
    }
}