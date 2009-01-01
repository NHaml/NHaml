using NUnit.Framework;

namespace NHaml.Tests
{
    [TestFixture]
    public class AssemblyLoaderTestFixture 
    {
        [Test]
        public void GetAssemblyList()
        {
            var list = AssemblyLoader.GetAssemblyList();
            Assert.IsNotEmpty(list);
        }
        [Test]
        public void Load()
        {
            var systemAssembly = AssemblyLoader.Load("System");
            Assert.IsNotNull(systemAssembly);
            Assert.IsTrue(systemAssembly.FullName.StartsWith("System,"));
            var systemDataAssembly = AssemblyLoader.Load("System.Data");
            Assert.IsNotNull(systemDataAssembly);
            Assert.IsTrue(systemDataAssembly.FullName.StartsWith("System.Data,"));
        }
    }
}
