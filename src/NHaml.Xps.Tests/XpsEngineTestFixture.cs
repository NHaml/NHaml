using System.Collections.Generic;
using System.IO;
using System.Printing;
using System.Threading;
using NHaml.TemplateResolution;
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
                    var sources = new List<IViewSource>{new FileViewSource(new FileInfo("XpsWithData.haml"))};
                    xpsHelper.PrintPreview(sources, "Hello");
                });
        }

        [Test]
        [Ignore]
        public void Print()
        {
            var xpsHelper = new XpsEngine();
            using (var autoResetEvent = new AutoResetEvent(false))
            {
                var sources = new List<IViewSource> { new FileViewSource(new FileInfo("XpsWithData.haml")) };
                xpsHelper.Print(sources, "Hello2", LocalPrintServer.GetDefaultPrintQueue, null, (sender, e) => autoResetEvent.Set());
                autoResetEvent.WaitOne();
            }
            Thread.Sleep(10000);
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
                        var sources = new List<IViewSource> { new FileViewSource(new FileInfo("XpsWithData.haml")) };
                                xpsHelper.Generate(sources, "Hello", tempTarget);
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