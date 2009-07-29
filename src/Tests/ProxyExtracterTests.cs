using System.Collections;
using System.Collections.Generic;
using Castle.DynamicProxy;
using NUnit.Framework;

namespace NHaml.Tests
{
    [TestFixture]
    public class ProxyExtracterTests
    {
        [Test]
        public void InterfaceProxyInGenericArgument()
        {
            var generator = new ProxyGenerator();

            var classProxy = generator.CreateClassProxy<Hashtable>();

            var genericType = typeof(List<>).MakeGenericType(new[] { classProxy.GetType() });
            var nonProxiedType = ProxyExtracter.GetNonProxiedType(genericType);
            Assert.AreEqual(typeof(List<Hashtable>), nonProxiedType);
        }
        [Test]
        public void ClassProxyInGenericArgument()
        {
            var generator = new ProxyGenerator();

            var classProxy = generator.CreateInterfaceProxyWithoutTarget<ICollection>();

            var genericType = typeof(List<>).MakeGenericType(new[] { classProxy.GetType() });
            var nonProxiedType = ProxyExtracter.GetNonProxiedType(genericType);
            Assert.AreEqual(typeof(List<ICollection>), nonProxiedType);
        }
        [Test]
        public void ClassProxy()
        {
            var generator = new ProxyGenerator();

            var classProxy = generator.CreateClassProxy<Hashtable>();

            var nonProxiedType = ProxyExtracter.GetNonProxiedType(classProxy.GetType());
            Assert.AreEqual(typeof(Hashtable), nonProxiedType);
        }
        [Test]
        public void InterfaceProxy()
        {
            var generator = new ProxyGenerator();

            var classProxy = generator.CreateInterfaceProxyWithoutTarget<ICollection>();

            var nonProxiedType = ProxyExtracter.GetNonProxiedType(classProxy.GetType());
        Assert.AreEqual(typeof(ICollection),nonProxiedType);
        }
    }
}