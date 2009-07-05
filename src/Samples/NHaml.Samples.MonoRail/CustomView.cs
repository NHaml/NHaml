using System.Diagnostics;
using System.Reflection;
using NHaml.Web.MonoRail;

namespace NHaml.Samples.MonoRail
{
    public class CustomView : NHamlMonoRailView
    {
        public string Version
        {
            get
            {
                var assemblyName = Assembly.GetExecutingAssembly().GetName();
                var codeBase = assemblyName.CodeBase;
                var filePath = codeBase.Remove(0, 8);
                var myFileVersionInfo = FileVersionInfo.GetVersionInfo(filePath);
                return myFileVersionInfo.ProductVersion;
            }
        }

    }
}
