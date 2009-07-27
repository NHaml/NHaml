using System.Diagnostics;
using System.IO;

namespace NHaml.Xps.Tests
{
    public class Previewer
    {
        public static void Run<TData>(TData context, string viewPath) where TData : class
        {
            var runner = new CrossThreadTestRunner();
            runner.RunInSTA(
                delegate
                    {
                        const string tempTarget = "temp.xps";
                        if (File.Exists(tempTarget))
                        {
                            File.Delete(tempTarget);
                        }
                        var xpsHelper = new XpsEngine();
                        xpsHelper.Generate(viewPath, context, tempTarget);
                        Process.Start(tempTarget);
                    });
        }
    }
}