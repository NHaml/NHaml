using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using NHaml.TemplateResolution;

namespace NHaml.Xps.Tests
{
    public class Previewer
    {
        public static void Run<TData>(TData context, IList<IViewSource> viewSources) where TData : class
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
                        xpsHelper.Generate(viewSources, context, tempTarget);
                        Process.Start(tempTarget);
                    });
        }
    }
}