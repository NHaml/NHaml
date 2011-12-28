using System;
using NHaml4.TemplateBase;
using NUnit.Framework;

namespace NHaml4.Tests
{
    [TestFixture]
    public class TemplateFactory_Tests
    {
        public class DummyTemplate : Template { }

        [Test]
        public void CreateTemplate_SimpleType_CreatesInstanceOfType()
        {
            Type t = typeof(DummyTemplate);
            var factory = new TemplateFactory(t);
            var actual = factory.CreateTemplate();
            Assert.IsInstanceOf<DummyTemplate>(actual);
        }

        [Test]
        public void Constructor_TypeWithoutConstructor_ThrowsInvalidTemplateTypeException()
        {
            Type t = typeof(string);
            Assert.Throws<InvalidTemplateTypeException>(() => new TemplateFactory(t));
        }
    }
}
