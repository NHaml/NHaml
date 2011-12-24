using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NHaml4.Tests
{
    [TestFixture]
    public class TemplateFactory_Tests
    {
        public class DummyTemplate : TemplateBase.Template { }

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
