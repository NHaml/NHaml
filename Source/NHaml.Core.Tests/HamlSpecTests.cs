using System.Diagnostics;
using NHaml.Core.Tests.Helpers;

namespace NHaml.Core.Tests
{
    public class HamlSpecTests
    {
        [HamlSpecTheory]
        public void Test(string fullName, string haml,string html)
        {
            Debug.WriteLine(fullName);
            Debug.WriteLine(haml);
            Debug.WriteLine("------------------------");
        }
    }
}