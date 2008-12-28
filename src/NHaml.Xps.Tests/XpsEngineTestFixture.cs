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
        public void Print()
        {
            var xpsHelper = new XpsEngine();
            xpsHelper.Print("XpsWithData.haml", "Hello2");
        }
        [Test]
        [Ignore]
        public void PrintAsync()
        {
            var xpsHelper = new XpsEngine();
            xpsHelper.PrintAsync("XpsWithData.haml", "Hello2");

            Thread.Sleep(50000);
        }
        [Test]
        [Ignore]
        public void PrintAsyncWithPrinter()
        {
            var xpsHelper = new XpsEngine();
            var printServer = new LocalPrintServer();
                xpsHelper.PrintAsync("XpsWithData.haml", "Hello2", LocalPrintServer.GetDefaultPrintQueue, null);
            Thread.Sleep(50000);
        }
        [Test]
        [Ignore]
        public void PrintAsyncWithPrinterName()
        {
            var xpsHelper = new XpsEngine();
                xpsHelper.PrintAsync("XpsWithData.haml", "Hello2", LocalPrintServer.GetDefaultPrintQueue().Name, null);
            Thread.Sleep(50000);
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
        [Test]
        public void WriteToFileAsync()
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
                            xpsHelper.GenerateAsync("XpsWithData.haml", "Hello", tempTarget, null);
                            Thread.Sleep(5000);
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
