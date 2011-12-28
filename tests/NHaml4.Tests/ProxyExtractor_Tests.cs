using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Moq;

namespace NHaml4.Tests
{
    public interface IDummyInterface { }

    [TestFixture]
    class ProxyExtractor_Tests
    {
        [Test]
        [TestCase(typeof(string), typeof(string))]
        [TestCase(typeof(int), typeof(int))]
        [TestCase(typeof(List<string>), typeof(List<string>))]
        public void GetNonProxiedType_NonProxiedTypes_ReturnsSameType(Type inputType, Type expectedResult)
        {
            var actualResult = ProxyExtracter.GetNonProxiedType(inputType);
            Assert.That(actualResult, Is.EqualTo(expectedResult));
        }

        [Test]
        public void GetNonProxiedType_ProxiedInterface_ReturnsInterface()
        {
            var inputType = new Mock<IDummyInterface>();
            var actualResult = ProxyExtracter.GetNonProxiedType(inputType.Object.GetType());
            Assert.That(actualResult, Is.EqualTo(typeof(IDummyInterface)));
        }

        [Test]
        public void GetNonProxiedType_ProxiedIList_ReturnsIList()
        {
            var inputType = new Mock<IList<string>>();
            var actualResult = ProxyExtracter.GetNonProxiedType(inputType.Object.GetType());
            Assert.That(actualResult, Is.EqualTo(typeof(IList<string>)));
        }
        

        //[Test]
        //public void TBC()
        //{
        //    int;
        //    Assert.Fail();
        //}
        
        //[Test]
        //public void TBC()
        //{
        //    ;
        //   this Assert.Fail();
        //}
    }
}
